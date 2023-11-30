namespace Game.Ecs.Core.Systems
{
    using System;
    using Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Components;
    
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public sealed class UpdateTransformDataSystem : IEcsRunSystem,IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsWorld _world;

        private UnityAspect _unityAspect;
        private EcsFilter _scaleFilter;
        private EcsFilter _rotationFilter;
        private EcsFilter _directionFilter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<TransformComponent>()
                .Inc<TransformPositionComponent>()
                .Exc<PrepareToDeathComponent>()
                .End();
            
            _scaleFilter = _world
                .Filter<TransformComponent>()
                .Inc<TransformScaleComponent>()
                .Exc<PrepareToDeathComponent>()
                .End();
            
            _rotationFilter = _world
                .Filter<TransformComponent>()
                .Inc<TransformRotationComponent>()
                .Exc<PrepareToDeathComponent>()
                .End();
            
            _directionFilter = _world
                .Filter<TransformComponent>()
                .Inc<TransformDirectionComponent>()
                .Exc<PrepareToDeathComponent>()
                .End();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var transformComponent = ref _unityAspect.Transform.Get(entity);
                ref var positionComponent = ref _unityAspect.Position.Get(entity);
                
                if (transformComponent.Value == null) continue;
                
                //==position
                var transform = transformComponent.Value;
                positionComponent.Position = transform.position;
                positionComponent.LocalPosition = transform.localPosition;
            }
            
            foreach (var entity in _scaleFilter)
            {
                ref var transformComponent = ref _unityAspect.Transform.Get(entity);
                if (transformComponent.Value == null) continue;
                var transform = transformComponent.Value;
  
                //==scale
                ref var scaleComponent = ref _unityAspect.Scale.Get(entity);
                scaleComponent.Scale = transform.lossyScale;
                scaleComponent.LocalScale = transform.localScale;
            }

            foreach (var entity in _rotationFilter)
            {
                ref var transformComponent = ref _unityAspect.Transform.Get(entity);
                if (transformComponent.Value == null) continue;
                var transform = transformComponent.Value;

                //==rotation

                ref var rotationComponent = ref _unityAspect.Rotation.Get(entity);

                rotationComponent.Euler = transform.eulerAngles;
                rotationComponent.Quaternion = transform.rotation;
                rotationComponent.LocalRotation = transform.localRotation;
            }
            
            foreach (var entity in _directionFilter)
            {
                ref var transformComponent = ref _unityAspect.Transform.Get(entity);
                if (transformComponent.Value == null) continue;
                var transform = transformComponent.Value;

                //==direction

                ref var directionComponent = ref _unityAspect.Direction.Get(entity);

                directionComponent.Forward = transform.forward;
                directionComponent.Right = transform.right;
                directionComponent.Up = transform.up;
            }
        }
    }
}