namespace Game.Ecs.Core.Converters
{
    using System;
    using System.Threading;
    using Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Components;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;

    [Serializable]
    public class TransformConverter : GameObjectConverter
    {
        public bool addPosition = true;
        public bool addDirection = true;
        
        protected override void OnApply(
            GameObject target,
            EcsWorld world,
            int entity, 
            CancellationToken cancellationToken = default)
        {
            ref var transformComponent = ref world.GetOrAddComponent<TransformComponent>(entity);
            
            if (addPosition)
            {
                ref var transformPositionComponent = ref world.GetOrAddComponent<TransformPositionComponent>(entity);
            }

            if (addDirection)
            {
                ref var transformDirectionComponent = ref world.GetOrAddComponent<TransformDirectionComponent>(entity);
            }
        }
    }
}