namespace UniGame.LeoEcs.Converter.Runtime
{
    using System;
    using System.Threading;
    using Abstract;
    using Core.Runtime.Extension;
    using Leopotam.EcsLite;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

#if TRI_INSPECTOR
    using TriInspector;
#endif
    
    [Serializable]
    [InlineProperty]
    public class ComponentConverterValue : IEcsComponentConverter
    {
        [SerializeReference]
        [InlineProperty]
        [ShowIf(nameof(IsSerializableConverter))]
        [HideLabel]
#if ODIN_INSPECTOR
        [FoldoutGroup("$GroupTitle",false)]
#endif
        public IEcsComponentConverter converter;
        
        [InlineEditor()] 
#if ODIN_INSPECTOR
        [FoldoutGroup("$GroupTitle",false)]
#endif
        [ShowIf(nameof(IsAssetConverter))]
        [HideLabel]
        public ComponentConverterAsset convertersAsset;

        [InlineEditor()] 
#if ODIN_INSPECTOR
        [FoldoutGroup("$GroupTitle",false)]
#endif
        [ShowIf(nameof(IsNestedConverter))]
        [HideLabel]
        public LeoEcsConverterAsset nesterConverter;

        public string GroupTitle => GetValueTitle();
        
        public bool IsEmpty => converter == null && 
                               convertersAsset == null &&
                               nesterConverter == null;
        
        public bool IsAssetConverter =>  IsEmpty || convertersAsset != null;
        
        public bool IsNestedConverter =>  IsEmpty || nesterConverter != null;

        public bool IsSerializableConverter => IsEmpty || converter!= null;

        public string Name => Value?.GetType().Name;
        
        public IEcsComponentConverter Value => GetValue();

        public IEcsComponentConverter GetValue()
        {
            if (converter != null) return converter;
            if (convertersAsset != null) return convertersAsset;
            if (nesterConverter != null) return nesterConverter;

            return null;
        }
        
        public string GetValueTitle()
        {
            if (converter != null) return converter.GetType().GetFormattedName();
            if (convertersAsset != null) return convertersAsset.name;
            if (nesterConverter != null) return nesterConverter.name;

            return "EMPTY";
        }
        
        public bool IsEnabled => Value?.IsEnabled ?? false;
        
        public void Apply(EcsWorld world, int entity)
        {
            if(Value.IsEnabled == false) return;
            Value.Apply(world,entity);
        }

        public bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return true;
            if (!string.IsNullOrEmpty(GroupTitle) && 
                GroupTitle.Contains(searchString,StringComparison.CurrentCultureIgnoreCase)) return true;
            var result = false;
#if ODIN_INSPECTOR
            result = Value?.IsMatch(searchString) ?? false;
#endif
            return result;
        }
    }
}