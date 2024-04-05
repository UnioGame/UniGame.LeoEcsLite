﻿namespace UniGame.LeoEcs.Bootstrap.Runtime
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    public class LeoEcsFeatureAsset : BaseLeoEcsFeature , ILeoEcsFeature
    {
        #region inspector

#if ODIN_INSPECTOR
        [InlineEditor()]
#endif
        public List<LeoEcsFeatureAsset> subFeatures = new List<LeoEcsFeatureAsset>();

        [SerializeReference]
        public List<ILeoEcsFeature> serializableFeatures = new List<ILeoEcsFeature>();

        #endregion

        public sealed override async UniTask InitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            if (!IsFeatureEnabled) return;
            
            foreach (var featureAsset in subFeatures)
                await featureAsset.InitializeFeatureAsync(ecsSystems);

            foreach (var ecsFeature in serializableFeatures)
                await ecsFeature.InitializeFeatureAsync(ecsSystems);

            await OnInitializeFeatureAsync(ecsSystems);
        }
        
        public override bool IsMatch(string searchString)
        {
            if (this == null) return false;
            
            if (string.IsNullOrEmpty(searchString)) return true;

            var typeName = GetType().Name;
            if (typeName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0) 
                return true;
            
            if (name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
                
            if (FeatureName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            foreach (var featureAsset in subFeatures)
            {
                if (featureAsset.IsMatch(searchString))
                    return true;
            }

#if ODIN_INSPECTOR
            foreach (var featureAsset in serializableFeatures)
            {
                if (featureAsset.IsMatch(searchString))
                    return true;
            }
#endif
            return false;
        }

        protected virtual UniTask OnInitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            return UniTask.CompletedTask;
        }
    }

    public class LeoEcsFeatureAssetT<TFeature> : LeoEcsFeatureAsset
        where TFeature : ILeoEcsFeature
    {
#if ODIN_INSPECTOR
        [HideLabel]
#endif
        [SerializeField] 
        public TFeature feature;

        protected override async UniTask OnInitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            await feature.InitializeFeatureAsync(ecsSystems);

        }
    }
}