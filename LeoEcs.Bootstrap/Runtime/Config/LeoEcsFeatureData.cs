namespace UniGame.LeoEcs.Bootstrap.Runtime.Config
{
    using System;
    using Cysharp.Threading.Tasks;
    using System.Collections.Generic;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;
    using Abstract;
    using UnityEngine;

    [Serializable]
    public class LeoEcsFeatureData : ILeoEcsFeature
    {
        [FoldoutGroup("$"+nameof(FeatureName))]
        [HideLabel]
        [ShowIf(nameof(UseAssetGroup))]
        public BaseLeoEcsFeature featureGroupAsset;

        [FoldoutGroup("$"+nameof(FeatureName))]
        [SerializeReference]
        [HideLabel]
        [ShowIf(nameof(UseSerializedGroup))]
        public ILeoEcsSystemsGroup featureGroup = null;

        public string FeatureName => Feature is null or EmptyFeature
            ? EmptyFeature.EcsEmptyFeatureName 
            : Feature.FeatureName;

        public bool IsEmptySerializedEmpty => featureGroup is null or EmptyFeature;
        
        public bool UseSerializedGroup => featureGroup != null && featureGroup is not EmptyFeature || 
                                          (featureGroupAsset == null && IsEmptySerializedEmpty);
        
        public bool UseAssetGroup => featureGroupAsset != null || 
                                     (featureGroupAsset == null && IsEmptySerializedEmpty);

        public ILeoEcsFeature Feature => UseAssetGroup 
            ? featureGroupAsset
            : featureGroup ;

        public bool IsFeatureEnabled => Feature is { IsFeatureEnabled: true };

        public IReadOnlyList<IEcsSystem> EcsSystems => Feature is not ILeoEcsSystemsGroup group
            ? EmptyFeature.EmptySystems 
            : group.EcsSystems;
        
        public bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return true;

            if (UseSerializedGroup && 
                !string.IsNullOrEmpty(FeatureName) &&
                FeatureName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (Feature != null && 
                Feature.GetType().Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)) 
                return true;

            if (EcsSystems == null)
                return false;
            
            foreach (var system in EcsSystems)
            {
                if (system == null) continue;
                    
                if (system is ISearchFilterable searchFilterable && searchFilterable.IsMatch(searchString))
                    return true;

                var typeName = system.GetType().Name;
                if (typeName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) 
                    return true;
            }

            return false;
        }

        public UniTask InitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            if(Feature == null) return UniTask.CompletedTask;
            return Feature.InitializeFeatureAsync(ecsSystems);
        }
    }
}
