namespace Game.Ecs.Core.Systems
{
    using Components;
    using Death.Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Shared.Extensions;

    public sealed class DestroyInvalidChildEntitiesSystem : IEcsRunSystem,IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsWorld _world;
        private EcsPool<KillRequest> _killPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world
                .Filter<OwnerDestroyedEvent>()
                .End();

            _killPool = _world.GetPool<KillRequest>();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
                _killPool.GetOrAddComponent(entity);
        }
    }
}