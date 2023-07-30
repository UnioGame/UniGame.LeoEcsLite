namespace Game.Ecs.Core.Systems
{
    using Components;
    using Leopotam.EcsLite;

    public sealed class CheckInvalidChildEntitiesSystem : IEcsRunSystem,IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsWorld _world;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world.Filter<OwnerComponent>().End();
        }
        
        public void Run(IEcsSystems systems)
        {

            var ownerPool = _world.GetPool<OwnerComponent>();
            var ownerDestroyedPool = _world.GetPool<OwnerDestroyedEvent>();

            foreach (var entity in _filter)
            {
                ref var ownerComponent = ref ownerPool.Get(entity);
                if(ownerComponent.Entity.Unpack(_world, out _))
                    continue;

                ownerDestroyedPool.Add(entity);
            }
        }
    }
}