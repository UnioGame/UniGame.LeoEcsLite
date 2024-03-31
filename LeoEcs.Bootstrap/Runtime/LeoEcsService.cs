﻿namespace UniGame.LeoEcs.Bootstrap.Runtime
{
    using UniGame.Core.Runtime;
    using UniGame.UniNodes.GameFlow.Runtime;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Abstract;
    using Config;
    using Converter.Runtime;
    using Core.Runtime.Extension;
    using Cysharp.Threading.Tasks;
    using LeoEcsLite.LeoEcs.Bootstrap.Runtime.Systems;
    using Leopotam.EcsLite;
    using PostInitialize;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.DataFlow;
    using Object = UnityEngine.Object;

    public class LeoEcsService : GameService,ILeoEcsService
    {
        private ILeoEcsSystemsConfig _config;
        private IEcsExecutorFactory _ecsExecutorFactory;
        private IEnumerable<ISystemsPlugin> _plugins;
        private Dictionary<string, IEcsSystems> _systemsMap;
        private Dictionary<string, ILeoEcsExecutor> _systemsExecutors;
        private IContext _context;

        private EcsWorld _world;
        private bool _isInitialized;
        private bool _ownThisWorld;
        private float _featureTimeout;

        private List<IEcsSystem> _startupSystems = new()
        {
            new InstantDestroySystem(),
        };
        
        private List<IEcsSystem> _lateSystems = new() {};

        private List<IEcsPostInitializeAction> _initializePlugins = new() 
        {
            new EcsDiPostInitialize(),
            new EcsProfileInitialize(),
        };

        public EcsWorld World => _world;

        public LeoEcsService(
            IContext context,
            EcsWorld world, 
            ILeoEcsSystemsConfig config,
            IEcsExecutorFactory ecsExecutorFactory, 
            IEnumerable<ISystemsPlugin> plugins,
            bool ownThisWorld,
            float featureTimeout)
        {
            _systemsMap = new Dictionary<string,IEcsSystems>(8);
            _systemsExecutors = new Dictionary<string, ILeoEcsExecutor>(8);

            _context = context;
            _world = world;
            _config = config;
            
            _ecsExecutorFactory = ecsExecutorFactory;
            _plugins = plugins;
            _ownThisWorld = ownThisWorld;
            _featureTimeout = featureTimeout;
            
            LifeTime.AddCleanUpAction(CleanUp);
        }
        
        public void SetDefaultWorld(EcsWorld world)
        {
            LeoEcsGlobalData.World = world;
        }
        
        public override async UniTask InitializeAsync()
        {
#if DEBUG
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
#endif
            await InitializeEcsService(_world);

            _isInitialized = true;

            ApplySystemsPlugins(_world);
            
            foreach (var systems in _systemsMap.Values)
            {
                systems.Init();
            }
            
#if DEBUG
            LogServiceTime("InitializeAsync",stopwatch);
#endif
        }

        public void Execute()
        {
            if (!_isInitialized) return;

            foreach (var (updateType, systems) in _systemsMap)
            {
                if (!_systemsExecutors.TryGetValue(updateType, out var executor))
                {
                    executor = _ecsExecutorFactory.Create(updateType);
                    _systemsExecutors[updateType] = executor;
                }

                executor.Execute(_world);
                executor.Add(systems);
            }

            ApplyPlugins(_world);
        }

        public void Pause()
        {
            foreach (var systemsExecutor in _systemsExecutors.Values)
                systemsExecutor.Stop();
        }

        public void CleanUp()
        {
            foreach (var systems in _systemsMap.Values)
                systems.Destroy();

            foreach (var ecsExecutor in _systemsExecutors.Values)
                ecsExecutor.Dispose();

            _systemsMap.Clear();
            _systemsExecutors.Clear();

            if (_ownThisWorld)
                _world?.Destroy();
            _world = null;
        }
        
        [Conditional("DEBUG")]
        private void LogServiceTime(string message, Stopwatch timer,bool stop = true)
        {
            var elapsed = timer.ElapsedMilliseconds;
            timer.Restart();
            if(stop) timer.Stop();
            GameLog.Log($"ECS FEATURE SOURCE: LOAD {message} TIME = {elapsed} ms");
        }

        private void ApplySystemsPlugins(EcsWorld world)
        {
            var groupIds = new List<string>();
            
            foreach (var systems in _systemsMap)
            {
                groupIds.Add(systems.Key);
            }
            
            foreach (var groupId in groupIds)
            {
                foreach (var plugin in _initializePlugins)
                {
                    var systemsSource = _systemsMap[groupId];
                    var newSystems = plugin.Apply(systemsSource,_context);
                    if (!newSystems.replace) continue;
                    
                    var systems = newSystems.value.GetAllSystems();
                    var systemsGroup = CreateEcsSystems(groupId, world);
                    foreach(var system in systems)
                        systemsGroup.Add(system);
                }
            }
        }
        
        private async UniTask InitializeEcsService(EcsWorld world)
        {
            var groups = _config
                .FeatureGroups
                .Select(x => CreateEcsGroupAsync(x,world));

            await UniTask.WhenAll(groups);
        }

        private List<ILeoEcsFeature> CollectFeatures(LeoEcsConfigGroup ecsGroup)
        {
            var features = new List<ILeoEcsFeature>();
            foreach (var feature in ecsGroup.features)
                features.Add(feature.Feature);
            return features;
        }
        
        private async UniTask CreateEcsGroupAsync(LeoEcsConfigGroup ecsGroup, EcsWorld world)
        {
            var systemsGroup = CollectFeatures(ecsGroup);
            await CreateEcsGroup(ecsGroup.updateType,world,systemsGroup);
        }

        private void ApplyPlugins(EcsWorld world)
        {
            foreach (var systemsPlugin in _plugins)
            {
                systemsPlugin.AddTo(LifeTime);
                
                foreach (var map in _systemsMap)
                    systemsPlugin.Add(map.Value);
                systemsPlugin.Execute(world);
            }
        }

        private IEcsSystems CreateEcsSystems(string groupId,EcsWorld world)
        {
            var systems = new EcsSystems(world,_context);
            _systemsMap[groupId] = systems;
            return systems;
        }
        
        private IEcsSystems CreateEcsSystems(EcsWorld world)
        {
            return new EcsSystems(world,_context);
        }
        
        private async UniTask CreateEcsGroup(
            string updateType, 
            EcsWorld world, 
            IReadOnlyList<ILeoEcsFeature> runnerFeatures)
        {
            if (!_systemsMap.TryGetValue(updateType, out var ecsSystems))
                ecsSystems = CreateEcsSystems(updateType,world);
                        
            foreach (var startupSystem in _startupSystems)
                ecsSystems.Add(startupSystem);

            var asyncFeatures = runnerFeatures
                .Select(x => InitializeFeatureAsync(ecsSystems, x));

            await UniTask.WhenAll(asyncFeatures);
            
            foreach (var startupSystem in _lateSystems)
                ecsSystems.Add(startupSystem);
        }

        public async UniTask InitializeFeatureAsync(IEcsSystems ecsSystems,ILeoEcsFeature feature)
        {
            if (!feature.IsFeatureEnabled) return;
                
#if DEBUG
            var timer = Stopwatch.StartNew();   
            timer.Restart();
#endif
            
            if (feature is ILeoEcsInitializableFeature initializeFeature)
            {
                var featureLifeTime = new LifeTimeDefinition();
                    
                await initializeFeature
                    .InitializeFeatureAsync(ecsSystems)
                    .AttachTimeoutLogAsync(GetErrorMessage(initializeFeature),_featureTimeout,featureLifeTime.Token);
                    
                featureLifeTime.Terminate();
            }
            
#if DEBUG
            LogServiceTime($"{feature.FeatureName} | {feature.GetType().Name}", timer,false);
#endif
                
            if(feature is not ILeoEcsSystemsGroup groupFeature)
                return;

            foreach (var system in groupFeature.EcsSystems)
            {
                var leoEcsSystem = system;

                //create instance of SO systems
                if (leoEcsSystem is Object systemAsset)
                {
                    systemAsset = Object.Instantiate(systemAsset);
                    leoEcsSystem = systemAsset as IEcsSystem;
                }
                
                var featureLifeTime = new LifeTimeDefinition();
                if (leoEcsSystem is ILeoEcsInitializableFeature initFeature)
                {
#if DEBUG
                    timer.Restart();
#endif
                    await initFeature
                        .InitializeFeatureAsync(ecsSystems)
                        .AttachTimeoutLogAsync(GetErrorMessage(initFeature),_featureTimeout,featureLifeTime.Token);
                    
#if DEBUG
                    LogServiceTime($"\tSUB FEATURE {feature.GetType().Name}", timer);
#endif
                    
                    featureLifeTime.Release();
                }

                ecsSystems.Add(leoEcsSystem);
            }
        }

        private string GetErrorMessage(ILeoEcsInitializableFeature feature)
        {
            var featureName = feature is ILeoEcsFeature ecsFeature
                ? ecsFeature.FeatureName
                : feature.GetType().Name;
            
            return $"ECS Feature Timeout Error for {featureName}";
        }
    }
    
    public struct EcsSystemsGroup
    {
        public string UpdateType;
        public IEcsSystems Systems;
    }
}