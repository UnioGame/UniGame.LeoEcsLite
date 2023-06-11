using System;
using System.Linq;

namespace UniGame.LeoEcs.Converter.Runtime
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;
    using Abstract;
    using UnityEngine;
    
    [CreateAssetMenu(menuName = "UniGame/LeoEcs/Converter/Ecs Converter")]
    public class LeoEcsConverterAsset : ScriptableObject,IComponentConverter,ILeoEcsGizmosDrawer
    {
        [BoxGroup("settings")]
        public bool enabled = true;
        
        [BoxGroup("settings")]
        public bool useConverters = false;
        
        [InlineProperty()]
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [ShowIf(nameof(useConverters))]
        public List<ComponentConverterValue> converters = new List<ComponentConverterValue>();

        public bool IsEnabled => enabled;
        
        public void Apply(EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            if (IsEnabled == false) return;
            
            OnApply(world,entity,cancellationToken);

            if (!useConverters) return;
            
            var converters = this.converters
                .Where(x => IsEnabled)
                .Select(x => x.Value);
            
            world.ApplyEcsComponents(entity,converters,cancellationToken);
        }

        public T GetOrAddConverter<T>() where T : class, IComponentConverter,new()
        {
            var converter = GetConverter<T>();
            if (converter != null) return converter;
            converter = new T();
            var converterValue = new ComponentConverterValue();
            converterValue.converter = converter;
            converters.Add(converterValue);
            return converter;
        }
        
        public T GetConverter<T>() where T : class, IComponentConverter
        {
            var converter = converters.FirstOrDefault(x => x.Value is T);
            return converter?.Value as T;
        }
        
        protected virtual void OnApply(EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            
        }

        public virtual bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return true;
            
            if(name.Contains(searchString,StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        public void DrawGizmos(GameObject target)
        {
            foreach (var converter in converters)
            {
                if(converter.Value is ILeoEcsGizmosDrawer gizmosDrawer)
                    gizmosDrawer.DrawGizmos(target);
            }
        }
    }
}