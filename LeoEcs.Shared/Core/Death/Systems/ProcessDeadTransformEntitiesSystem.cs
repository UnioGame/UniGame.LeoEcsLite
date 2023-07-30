namespace Game.Ecs.Core.Death.Systems
{
    using Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Shared.Components;
    using Object = UnityEngine.Object;
    
    public sealed class ProcessDeadTransformEntitiesSystem : IEcsRunSystem,IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsWorld _world;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _filter = _world
                .Filter<DestroyComponent>()
                .Inc<TransformComponent>()
                .End();
        }
        
        public void Run(IEcsSystems systems)
        {
            var transformPool = _world.GetPool<TransformComponent>();
            
            foreach (var entity in _filter)
            {
                ref var transformComponent = ref transformPool.Get(entity);
                var transform = transformComponent.Value;
                
                if(transform && transform.gameObject)
                    Object.Destroy(transform.gameObject);
                
                _world.DelEntity(entity);
            }
        }
    }
}