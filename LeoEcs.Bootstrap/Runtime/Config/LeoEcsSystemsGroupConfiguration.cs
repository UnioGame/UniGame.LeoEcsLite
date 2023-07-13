using Cysharp.Threading.Tasks;

namespace UniGame.LeoEcs.Bootstrap.Runtime.Config
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UnityEngine.Serialization;

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
        private List<IEcsSystem> _systems = new List<IEcsSystem>();
        
        [Searchable]
        [InlineEditor()]
        public List<BaseLeoEcsFeature> nestedFeatures = new List<BaseLeoEcsFeature>();

        [Searchable]
        [SerializeReference]
        public List<ILeoEcsFeature> serializableFeatures = new List<ILeoEcsFeature>();

        
        public string FeatureName => name;
        
        public bool IsFeatureEnabled => _active;

        public IReadOnlyList<IEcsSystem> EcsSystems => _systems;
        
        
        public async UniTask InitializeFeatureAsync(EcsSystems ecsSystems)
        {
            if (!IsFeatureEnabled) return;

            foreach (var featureAsset in nestedFeatures)
            {
                if(!featureAsset.IsFeatureEnabled) continue; 
                await featureAsset.InitializeFeatureAsync(ecsSystems);
            }

            foreach (var ecsFeature in serializableFeatures)
            {
                if(!ecsFeature.IsFeatureEnabled) continue; 
                await ecsFeature.InitializeFeatureAsync(ecsSystems);
            }

            await OnInitializeFeatureAsync(ecsSystems);
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

        protected virtual UniTask OnInitializeFeatureAsync(EcsSystems ecsSystems)
        {
            return UniTask.CompletedTask;
        }
    }
}