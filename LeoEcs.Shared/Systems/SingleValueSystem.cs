namespace UniGame.LeoEcsLite.LeoEcs.Shared.Systems
{
    using Leopotam.EcsLite;

    public class SingleValueSystem<TComponent> : IEcsInitSystem,IEcsRunSystem 
        where TComponent : struct
    {
        private EcsFilter _filter;
        private EcsPool<TComponent> _pool;
        private EcsWorld _world;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world.Filter<TComponent>().End();
            _pool = _world.GetPool<TComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var component = ref _pool.Get(entity);
                var packed = _world.PackEntity(entity);
            }
        }
    }
}
