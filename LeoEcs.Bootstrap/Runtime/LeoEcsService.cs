namespace UniGame.LeoEcs.Bootstrap.Runtime
{
    using UniGame.Core.Runtime;
    using UniGame.UniNodes.GameFlow.Runtime;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Abstract;
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
        private readonly ILeoEcsSystemsConfig _config;
        private readonly IEcsExecutorFactory _ecsExecutorFactory;
        private readonly IEnumerable<ISystemsPlugin> _plugins;
        private readonly bool _ownThisWorld;
        private readonly float _featureTimeout;
        private readonly Dictionary<string, EcsSystems> _systemsMap;
        private readonly Dictionary<string, ILeoEcsExecutor> _systemsExecutors;
        private readonly IContext _context;
        
        private EcsWorld _world;
        private bool _isInitialized;

        private List<IEcsSystem> _startupSystems = 
            new List<IEcsSystem>()
        {
            new InstantDestroySystem(),
        };
        
        private List<IEcsSystem> _lateSystems = new List<IEcsSystem>() {};

        private List<IEcsPostInitializeAction> _postInitializeActions = 
            new List<IEcsPostInitializeAction>() 
        {
            new EcsDiPostInitialize(),
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
            _systemsMap = new Dictionary<string, EcsSystems>(8);
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
            LeoEcsConvertersData.World = world;
        }
        
        public override async UniTask InitializeAsync()
        {
            await InitializeEcsService(_world);

            _isInitialized = true;

            foreach (var systems in _systemsMap.Values)
            {
                systems.Init();
                ApplyPostInitialize(systems);
            }
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

        private void ApplyPostInitialize(IEcsSystems ecsSystems)
        {
            foreach (var postInitializeAction in _postInitializeActions)
            {
                foreach (var system in ecsSystems.GetAllSystems())
                    postInitializeAction.Apply(ecsSystems,system);
            }
        }
        
        private async UniTask InitializeEcsService(EcsWorld world)
        {
            var features = new List<ILeoEcsFeature>();
            
            foreach (var updateGroup in _config.FeatureGroups)
            {
                features.Clear();
                foreach (var feature in updateGroup.features)
                    features.Add(feature.Feature);
                
                await CreateEcsGroupRunner(updateGroup.updateType, world, features);
            }
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

        private async UniTask CreateEcsGroupRunner(string updateType, EcsWorld world, IReadOnlyList<ILeoEcsFeature> runnerFeatures)
        {
            if (!_systemsMap.TryGetValue(updateType, out var ecsSystems))
            {
                ecsSystems = new EcsSystems(world,_context);
                _systemsMap[updateType] = ecsSystems;
            }
                        
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
                    .AttachTimeoutLogAsync(GetErrorMessage(initializeFeature),_featureTimeout,featureLifeTime.CancellationToken);
                    
                featureLifeTime.Terminate();
            }
            
#if DEBUG
            var elapsed = timer.ElapsedMilliseconds;
            timer.Stop();
            GameLog.LogRuntime($"ECS FEATURE SOURCE: LOAD TIME {feature.FeatureName} | {feature.GetType().Name} = {elapsed} ms");
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

                if (leoEcsSystem is ILeoEcsInitializableFeature initFeature)
                {
                    var featureLifeTime = new LifeTimeDefinition();
                    await initFeature.InitializeFeatureAsync(ecsSystems)
                        .AttachTimeoutLogAsync(GetErrorMessage(initFeature),_featureTimeout,featureLifeTime.CancellationToken);
                    featureLifeTime.Terminate();
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
}