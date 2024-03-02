namespace UniGame.LeoEcs.Bootstrap.Runtime
{
    using System;
    using Abstract;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;

    [Serializable]
    public abstract class LeoEcsFeature : ILeoEcsFeature
    {
        public string name;
        public bool isEnabled = true;

        public virtual bool IsFeatureEnabled => isEnabled;

        public virtual string FeatureName => string.IsNullOrEmpty(name) ? GetType().Name : name;

        public async UniTask InitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            if (!isEnabled) return;
            await OnInitializeFeatureAsync(ecsSystems);
        }

        public virtual bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return true;
            
            if (!string.IsNullOrEmpty(name) && ContainsSearchString(name,searchString)) 
                return true;
            
            return ContainsSearchString(FeatureName, searchString);
        }

        protected abstract UniTask OnInitializeFeatureAsync(IEcsSystems ecsSystems);

        protected bool ContainsSearchString(string source, string filter)
        {
            return !string.IsNullOrEmpty(source) && 
                   source.Contains(filter, StringComparison.OrdinalIgnoreCase);
        }
        
    }
}