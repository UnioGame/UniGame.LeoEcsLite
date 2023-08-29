using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UniGame.LeoEcs.Bootstrap.Runtime
{
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using UniGame.Core.Runtime.ScriptableObjects;
    using Abstract;

#if UNITY_EDITOR
    using UnityEditor;
    using UniModules.Editor;
#endif
    
    public abstract class BaseLeoEcsFeature : LifetimeScriptableObject,ILeoEcsFeature
    {
        [ShowIf(nameof(ShowFeatureInfo))]
        [SerializeField]
        public bool isEnabled = true;

        public virtual bool IsFeatureEnabled => isEnabled;

        public virtual string FeatureName => name;

        public virtual bool ShowFeatureInfo => true;
        
        public abstract UniTask InitializeFeatureAsync(IEcsSystems ecsSystems);

        public virtual bool IsMatch(string searchString)
        {
            if (this == null) return false;
            
            if (string.IsNullOrEmpty(searchString)) return true;
            
            if (!string.IsNullOrEmpty(name) && ContainsSearchString(name,searchString)) 
                return true;
            
            return ContainsSearchString(FeatureName, searchString);
        }

        protected bool ContainsSearchString(string source, string filter)
        {
            return !string.IsNullOrEmpty(source) && 
                   source.Contains(filter, StringComparison.OrdinalIgnoreCase);
        }

        [Button]
        private void Save()
        {
#if UNITY_EDITOR
            this.SaveAsset();
#endif
        }
        
    }
}