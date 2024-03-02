namespace UniGame.LeoEcs.Bootstrap.Runtime.Config
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;

    [Serializable]
    public class EmptyFeature : ILeoEcsSystemsGroup
    {
        public static readonly List<IEcsSystem> EmptySystems = new();
        public const string EcsEmptyFeatureName = "Empty Feature";
        
        [ReadOnly]
        public string name = EcsEmptyFeatureName;
        
        public UniTask InitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            return UniTask.CompletedTask;
        }

        public bool IsMatch(string searchString)
        {
            return string.IsNullOrEmpty(searchString);
        }

        public bool IsFeatureEnabled => false;

        public string FeatureName => EcsEmptyFeatureName;
        public IReadOnlyList<IEcsSystem> EcsSystems => EmptySystems;
    }
}