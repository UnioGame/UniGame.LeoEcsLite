namespace UniGame.LeoEcs.Converter.Runtime.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Abstract;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using Shared.Components;
    using Shared.Extensions;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class MonoLeoEcsGroupConverter : MonoLeoEcsConverter<EcsComponentsGroupConverter>
    {
    }

    [Serializable]
    public class EcsComponentsGroupConverter : LeoEcsConverter
    {
        [SerializeField]
        private string groupName;
        
        [InlineProperty]
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