namespace UniGame.LeoEcs.Bootstrap.Runtime
{
    using System;
    using System.Linq;
    using Abstract;
    using Config;
    using Converter.Runtime;
    using Core.Runtime;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using GameFlow.Runtime.Services;
    using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if TRI_INSPECTOR
    using TriInspector;
#endif
    
    [Serializable]
    [CreateAssetMenu(menuName = "UniGame/LeoEcs/EcsSystemsSource", fileName = nameof(LeoEcsServiceSource))]
    public class LeoEcsServiceSource : ServiceDataSourceAsset<ILeoEcsService>,IEcsExecutorFactory
    {
        #region inspector

        /// <summary>
        /// timeout in ms for feature initialization
        /// </summary>
        [Tooltip("timeout in ms for feature initialization")]
        public float featureTimeout = 20000f;

        [InlineEditor] 
        public LeoEcsFeaturesConfiguration ecsConfiguration;

#if ODIN_INSPECTOR
        [InlineEditor]
#endif
        [Space(10)]
        [SerializeField]
        public LeoEcsUpdateMapAsset updatesMap;

#if ODIN_INSPECTOR
        [InlineEditor]
        [ShowIf(nameof(IsRuntimeConfigVisible))]
#endif
        [Space(10)]
        [SerializeField]
        public LeoEcsFeaturesConfiguration _runtimeConfiguration;

        #endregion

        private LeoEcsUpdateMapAsset _updateMapData;
        
        public bool IsRuntimeConfigVisible => Application.isPlaying && _runtimeConfiguration != null;

        protected override async UniTask<ILeoEcsService> CreateServiceInternalAsync(IContext context)
        {
            LeoEcsGlobalData.World = null;

            var config = Instantiate(ecsConfiguration);
            _updateMapData = Instantiate(updatesMap);
            _runtimeConfiguration = config;

            var plugins = _updateMapData
                .systemsPlugins
                .Select(x => x.Create())
                .ToList();

            var worldConfig = config.worldConfiguration.Create();
            var world = new EcsWorld(in worldConfig);
            
            context.Publish(world);
            
            var ecsService = new LeoEcsService(context,world, 
                config,
                this, 
                plugins,
                true,featureTimeout);
            
            //start ecs service update
            await ecsService.InitializeAsync();
            ecsService.Execute();
            ecsService.SetDefaultWorld(world);

            var assetName = name;
            
#if UNITY_EDITOR
            LifeTime.LogOnRelease($"SERVICE: LeoEcs Service COMPLETE : {assetName}",Color.yellow);
#endif
            context.LifeTime.AddDispose(ecsService);
            return ecsService;
        }
        
        public ILeoEcsExecutor Create(string updateId)
        {
            foreach (var updateOrder in _updateMapData.updateQueue)
            {
                if (updateOrder.OrderId.Equals(updateId, StringComparison.OrdinalIgnoreCase))
                    return updateOrder.Factory.Create();
            }

            return _updateMapData.defaultFactory?.Create();
        }

    }
}