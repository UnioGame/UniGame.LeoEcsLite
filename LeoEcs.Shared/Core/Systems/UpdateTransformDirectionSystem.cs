namespace Game.Ecs.Core.Systems
{
    using System;
    using Components;
    using Leopotam.EcsLite;
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
    public sealed class UpdateTransformDirectionSystem : IEcsRunSystem,IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsWorld _world;
        
        private EcsPool<TransformComponent> _transformPool;
        private EcsPool<TransformDirectionComponent> _directionsPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<TransformComponent>()
                .Inc<TransformDirectionComponent>()
                .Exc<PrepareToDeathComponent>()
                .End();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var transformComponent = ref _transformPool.Get(entity);
                
                if(transformComponent.Value == null) continue;
                
                ref var positionComponent = ref _directionsPool.Get(entity);
                
                var transform = transformComponent.Value;
                positionComponent.Forward = transform.forward;
                positionComponent.Right = transform.right;
                positionComponent.Up = transform.up;
            }
        }
    }
}