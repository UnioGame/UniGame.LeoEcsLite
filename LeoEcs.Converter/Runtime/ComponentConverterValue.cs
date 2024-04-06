namespace UniGame.LeoEcs.Converter.Runtime
{
    using System;
    using Abstract;
    using Leopotam.EcsLite;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

#if TRI_INSPECTOR
    using TriInspector;
#endif
    
    
#if  ODIN_INSPECTOR || TRI_INSPECTOR
    [InlineProperty]
#endif
    [Serializable]
    public class ComponentConverterValue : IEcsComponentConverter
    {
        [SerializeReference]
#if  ODIN_INSPECTOR || TRI_INSPECTOR
        [InlineProperty]
        [ShowIf(nameof(IsSerializableConverter))]
        [HideLabel]
#endif
#if ODIN_INSPECTOR
        [FoldoutGroup("$GroupTitle",false)]
#endif
        public IEcsComponentConverter converter;
        
#if  ODIN_INSPECTOR || TRI_INSPECTOR
        [InlineEditor()] 
#endif
#if ODIN_INSPECTOR
        [FoldoutGroup("$GroupTitle",false)]
#endif
#if  ODIN_INSPECTOR || TRI_INSPECTOR
        [ShowIf(nameof(IsAssetConverter))]
        [HideLabel]
#endif
        public ComponentConverterAsset convertersAsset;

        
#if  ODIN_INSPECTOR || TRI_INSPECTOR
        [InlineEditor] 
        [ShowIf(nameof(IsNestedConverter))]
        [HideLabel]
#endif
#if ODIN_INSPECTOR
        [FoldoutGroup("$GroupTitle",false)]
#endif
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