namespace UniGame.LeoEcs.Converter.Runtime.Converters
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Leopotam.EcsLite;
    using Shared.Components;
    using Shared.Extensions;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

#if TRI_INSPECTOR
    using TriInspector;
#endif
    
    public class MonoLeoEcsGroupConverter : MonoLeoEcsConverter<EcsComponentsGroupConverter>
    {
    }

    [Serializable]
    public class EcsComponentsGroupConverter : LeoEcsConverter
    {
        [SerializeField]
        private string groupName;
        
#if  ODIN_INSPECTOR || TRI_INSPECTOR
        [InlineProperty]
#endif
        [SerializeReference]
        private List<IEcsComponentConverter> _converters = new();

        public override void Apply(GameObject target, EcsWorld world, int entity)
        {
            ref var gameObjectComponent = ref world
                .GetOrAddComponent<GameObjectComponent>(entity);
            gameObjectComponent.Value = target;
            
            foreach (var converter  in _converters)
                converter.Apply(world, entity);
        }
    }
}