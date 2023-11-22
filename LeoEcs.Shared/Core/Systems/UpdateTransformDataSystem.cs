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

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<TransformComponent>()
                .Inc<TransformPositionComponent>()
                .Exc<PrepareToDeathComponent>()
                .End();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var transformComponent = ref _unityAspect.Transform.Get(entity);
                
                if (transformComponent.Value == null) continue;
                
                //==position
                
                ref var positionComponent = ref _unityAspect.Position.Get(entity);
                
                var transform = transformComponent.Value;
                positionComponent.Position = transform.position;
                positionComponent.LocalPosition = transform.localPosition;
                
                //==scale
                
                ref var scaleComponent = ref _unityAspect.Scale.Get(entity);
                
                scaleComponent.Scale = transform.lossyScale;
                scaleComponent.LocalScale = transform.localScale;
                
                //==rotation
                
                ref var rotationComponent = ref _unityAspect.Rotation.Get(entity);
                
                rotationComponent.Euler = transform.eulerAngles;
                rotationComponent.Quaternion = transform.rotation;
                rotationComponent.LocalRotation = transform.localRotation;
                
                //==direction
                
                ref var directionComponent = ref _unityAspect.Direction.Get(entity);
                
                directionComponent.Forward = transform.forward;
                directionComponent.Right = transform.right;
                directionComponent.Up = transform.up;
            }
        }
    }
}