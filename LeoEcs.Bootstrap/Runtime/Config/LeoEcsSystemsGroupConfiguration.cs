using Cysharp.Threading.Tasks;

namespace UniGame.LeoEcs.Bootstrap.Runtime.Config
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Abstract;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;
    using UniCore.Runtime.ProfilerTools;
    using UnityEngine;
    using UnityEngine.Serialization;
    using Object = UnityEngine.Object;

    [Serializable]
    public class LeoEcsSystemsGroupConfiguration : ILeoEcsSystemsGroup
    {
        private const string FeatureInfoGroup = "feature info";
        
        public bool showFeatureInfo = true;
        
        [FormerlySerializedAs("_name")]
        [BoxGroup(FeatureInfoGroup)]
        [ShowIf(nameof(showFeatureInfo))]
        [SerializeField]
        public string name;
        
        [BoxGroup(FeatureInfoGroup)]
        [ShowIf(nameof(showFeatureInfo))]
        [SerializeField]
        public bool _active = true;
 
        /// <summary>
        /// ecs group systems
        /// </summary>
#if ODIN_INSPECTOR
        [InlineProperty]
#endif
        [SerializeReference]
        [Searchable]
        private List<IEcsSystem> _systems = new();
        
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [InlineEditor()]
        public List<BaseLeoEcsFeature> nestedFeatures = new();

        [Searchable]
        [SerializeReference]
        public List<ILeoEcsFeature> serializableFeatures = new();

        
        public string FeatureName => name;
        
        public bool IsFeatureEnabled => _active;

        public IReadOnlyList<IEcsSystem> EcsSystems => _systems;
        
        
        public async UniTask InitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            if (!IsFeatureEnabled) return;

#if DEBUG
            var timer = Stopwatch.StartNew();   
            timer.Restart();
#endif
            
            foreach (var featureAsset in nestedFeatures)
            {
                if(!featureAsset.IsFeatureEnabled) continue;
                var featureInstance = Object.Instantiate(featureAsset);
#if DEBUG  
                timer.Restart();
#endif
                
                await featureInstance.InitializeFeatureAsync(ecsSystems);

#if DEBUG
                var elapsed = timer.ElapsedMilliseconds;
                timer.Stop();
                GameLog.Log($"\tECS SUB FEATURE SOURCE: LOAD TIME {featureInstance.FeatureName} | {featureInstance.GetType().Name} = {elapsed} ms");
#endif
            }

            foreach (var ecsFeature in serializableFeatures)
            {
                if(!ecsFeature.IsFeatureEnabled) continue; 
#if DEBUG  
                timer.Restart();
#endif
                await ecsFeature.InitializeFeatureAsync(ecsSystems);
#if DEBUG
                var elapsed = timer.ElapsedMilliseconds;
                timer.Stop();
                GameLog.Log($"ECS FEATURE SOURCE: LOAD TIME {ecsFeature.FeatureName} | {ecsFeature.GetType().Name} = {elapsed} ms");
#endif
            }

#if DEBUG  
            timer.Restart();
#endif
            
            await OnInitializeFeatureAsync(ecsSystems);

#if DEBUG
            var elapsedTime = timer.ElapsedMilliseconds;
            timer.Stop();
            GameLog.Log($"ECS FEATURE SOURCE: LOAD TIME SELF {FeatureName} | {this.GetType().Name} = {elapsedTime} ms");
#endif
        }
        
        public virtual bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return true;

            var typeName = GetType().Name;
            if (typeName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0) 
                return true;

            if (FeatureName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            foreach (var featureAsset in nestedFeatures)
            {
                if (featureAsset.IsMatch(searchString))
                    return true;
            }

            foreach (var featureAsset in serializableFeatures)
            {
                if (featureAsset.IsMatch(searchString))
                    return true;
            }
            
            return false;
        }

        protected virtual UniTask OnInitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            return UniTask.CompletedTask;
        }
    }
}