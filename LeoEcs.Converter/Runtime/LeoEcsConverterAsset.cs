using System;
using System.Linq;

namespace UniGame.LeoEcs.Converter.Runtime
{
    using System.Collections.Generic;
    using System.Threading;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;
    using Abstract;
    using UniModules.UniCore.Runtime.Utils;
    using UnityEngine;

#if UNITY_EDITOR
    using UniModules.Editor;
#endif
    
    [CreateAssetMenu(menuName = "UniGame/LeoEcs/Converter/Ecs Converter",fileName = "Ecs Converter Asset")]
    public class LeoEcsConverterAsset : ScriptableObject,
        IEcsComponentConverter,
        ILeoEcsGizmosDrawer, 
        IEcsConverterProvider,
        IConverterEntityDestroyHandler
    {
        [BoxGroup("settings")]
        public bool enabled = true;
        
        [BoxGroup("settings")]
        public bool useConverters = false;
        
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [ShowIf(nameof(useConverters))]
        public List<ComponentConverterValue> converters = new List<ComponentConverterValue>();

        public bool IsEnabled => enabled;

        public string Name => this.GetType().Name;

        public IEnumerable<IEcsComponentConverter> Converters => converters;
        
        public void Apply(EcsWorld world, int entity)
        {
            if (IsEnabled == false) return;
            
            OnApply(world,entity);

            if (!useConverters) return;
            
            var converters = this.converters
                .Where(x => IsEnabled)
                .Select(x => x.Value);
            
            world.ApplyEcsComponents(entity,converters);
        }

        public IEcsComponentConverter GetOrAddConverter(Type converterType)
        {
            var converter = GetConverter(converterType);
            if (converter != null) return converter;
            
            converter = converterType.CreateWithDefaultConstructor() as IEcsComponentConverter;
            var converterValue = new ComponentConverterValue();
            converterValue.converter = converter;
            converters.Add(converterValue);
            return converter;
        }
        
        public void OnEntityDestroy(EcsWorld world, int entity)
        {
            foreach (var converter in converters)
            {
                if(converter.Value is IConverterEntityDestroyHandler destroyHandler)
                    destroyHandler.OnEntityDestroy(world, entity);
            }
        }
        
        public bool AddConverter(IEcsComponentConverter converter, bool replace = false)
        {
            var sourceConverter = GetConverter(converter.GetType());
            if (sourceConverter != null && replace == false) return false;

            if (sourceConverter != null)
                converters.RemoveAll(x => x.Value == sourceConverter);

            var converterValue = new ComponentConverterValue();
            converterValue.converter = converter;
            converters.Add(converterValue);
            return true;
        }
        
        public T GetOrAddConverter<T>() where T : class, IEcsComponentConverter,new()
        {
            var converter = GetConverter<T>();
            if (converter != null) return converter;
            converter = new T();
            var converterValue = new ComponentConverterValue();
            converterValue.converter = converter;
            converters.Add(converterValue);
            return converter;
        }
        
        public void RemoveConverter<T>() where T : IEcsComponentConverter
        {
            converters.RemoveAll(x => x.Value is T);
            
            foreach (var converter in converters)
            {
                if(converter.Value is not IEcsConverterProvider converterProvider)
                    continue;
                converterProvider.RemoveConverter<T>();
            }
        }
        
        public IEcsComponentConverter GetConverter(Type target)
        {
            var result = default(IEcsComponentConverter);
            foreach (var converter in converters)
            {
                var converterValue = converter.Value;
                if(converterValue == null) continue;
                
                if (converterValue.GetType() == target)
                {
                    result = converterValue;
                    break;
                }
                
                if(converterValue is IEcsConverterProvider converterProvider)
                    result = converterProvider.GetConverter(target);
                
                if (result != null) break;
            }

            return result;
        }
        
        public T GetConverter<T>() where T : class
        {
            var result = default(T);
            foreach (var converterValue in converters)
            {
                result = converterValue.Value switch
                {
                    T converter => converter,
                    IEcsConverterProvider converterProvider => converterProvider.GetConverter<T>(),
                    _ => null
                };
                
                if (result != null) break;
            }

            return result;
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