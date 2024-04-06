using System;
using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

#if TRI_INSPECTOR
    using TriInspector;
#endif

namespace UniGame.LeoEcs.Bootstrap.Runtime
{
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using Abstract;

#if UNITY_EDITOR
    using UniModules.Editor;
#endif
    
    public class BaseLeoEcsFeature : ScriptableObject, ILeoEcsFeature
    {
        public const string DefaultFeatureName = "EMPTY Feature";
        
#if ODIN_INSPECTOR || TRI_INSPECTOR
        [ShowIf(nameof(ShowFeatureInfo))]
#endif
        [SerializeField]
        public bool isEnabled = true;

        public virtual bool IsFeatureEnabled => isEnabled;

        public virtual string FeatureName => this==null || string.IsNullOrEmpty(name) ? DefaultFeatureName : name;

        public virtual bool ShowFeatureInfo => true;

        public virtual UniTask InitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            return UniTask.CompletedTask;
        }

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
            this.MarkDirty();
            this.SaveAsset();
#endif
        }
        
    }
}