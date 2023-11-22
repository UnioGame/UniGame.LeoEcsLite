using UniGame.LeoEcs.Shared.Extensions;

namespace UniGame.LeoEcs.Converter.Runtime.Converters
{
    using System;
    using System.Threading;
    using LeoEcsLite.LeoEcs.Shared.Components;
    using Leopotam.EcsLite;
    using Shared.Components;
    using UnityEngine;

    [Serializable]
    public class BaseGameObjectComponentConverter : LeoEcsConverter
    {
        public sealed override void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            ref var transformComponent = ref world.GetOrAddComponent<TransformComponent>(entity);
            ref var gameObjectComponent = ref world.GetOrAddComponent<GameObjectComponent>(entity);
            ref var objectComponent = ref world.GetOrAddComponent<UnityObjectComponent>(entity);

            ref var transformPositionComponent = ref world.GetOrAddComponent<TransformPositionComponent>(entity);
            ref var directionComponent = ref world.GetOrAddComponent<TransformDirectionComponent>(entity);
            ref var scaleComponent = ref world.GetOrAddComponent<TransformScaleComponent>(entity);
            ref var rotationComponent = ref world.GetOrAddComponent<TransformRotationComponent>(entity);

            var transform = target.transform;
            transformComponent.Value = transform;
            gameObjectComponent.Value = target;
            objectComponent.Value = target;
            transformPositionComponent.Position = transform.position;
            directionComponent.Forward = transform.forward;
            directionComponent.Up = transform.up;
            directionComponent.Right = transform.right;
            scaleComponent.Scale = transform.lossyScale;
            scaleComponent.LocalScale = transform.localScale;

            rotationComponent.Quaternion = transform.rotation;
            rotationComponent.LocalRotation = transform.localRotation;
            rotationComponent.Euler = transform.eulerAngles;
        }
    }
}