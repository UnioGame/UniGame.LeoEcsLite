namespace Game.Ecs.Core.Death.Systems
{
    using Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Shared.Components;

    public sealed class DisableColliderSystem : IEcsRunSystem,IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsWorld _world;
        private EcsPool<ColliderComponent> _colliderPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<ColliderComponent>()
                .Inc<DisabledEvent>()
                .End();
            
            _colliderPool = _world.GetPool<ColliderComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            

            foreach (var entity in _filter)
            {
                ref var collider = ref _colliderPool.Get(entity);
                collider.Value.enabled = false;
            }
        }
    }
}