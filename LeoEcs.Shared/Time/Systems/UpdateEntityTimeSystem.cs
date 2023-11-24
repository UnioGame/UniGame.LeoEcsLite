namespace Game.Ecs.Time.Systems
{
    using Components;
    using Leopotam.EcsLite;
    using Service;
    using UnityEngine;

    public class UpdateEntityTimeSystem : IEcsRunSystem,IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsWorld _world;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world.Filter<EntityGameTimeComponent>().End();
        }
        
        public void Run(IEcsSystems systems)
        {
            var gameTimePool = _world.GetPool<EntityGameTimeComponent>();

            GameTime.Time = Time.time;
            GameTime.DeltaTime = Time.deltaTime;
            GameTime.RealTime = Time.realtimeSinceStartup;
            
            foreach (var entity in _filter)
            {
                ref var timeComponent = ref gameTimePool.Get(entity);
                timeComponent.Value += GameTime.DeltaTime;
            }
        }
    }
}
