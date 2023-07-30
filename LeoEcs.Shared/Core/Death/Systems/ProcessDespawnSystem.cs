namespace Game.Ecs.Core.Death.Systems
{
    using Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Shared.Components;
    using UniGame.LeoEcs.Shared.Extensions;
    using UniGame.Runtime.ObjectPool.Extensions;
    using UnityEngine;

    public sealed class ProcessDespawnSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsWorld _world;
        
        private EcsFilter _filter;
        private EcsFilter _eventFilter;
        

        private EcsPool<DeadEvent> _deadEventPool;
        private EcsPool<TransformComponent> _transformPool;
        private EcsPool<PoolingComponent> _pooledPool;
        private EcsPool<DontKillComponent> _dontKillPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _eventFilter = _world
                .Filter<KillEvent>()
                .End();

            _transformPool = _world.GetPool<TransformComponent>();
            _deadEventPool = _world.GetPool<DeadEvent>();

            _pooledPool = _world.GetPool<PoolingComponent>();
            _dontKillPool = _world.GetPool<DontKillComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var killEventEntity in _eventFilter)
            {
                ref var killEvent = ref _world
                    .GetComponent<KillEvent>(killEventEntity);
                
                if(!killEvent.Destination.Unpack(_world,out var killedEntity))
                    continue;
                
                if(!_pooledPool.Has(killedEntity) || _dontKillPool.Has(killedEntity))
                   continue;

                var isTransform = _transformPool.Has(killedEntity);
                Transform transform = null;

                if (isTransform)
                {
                    ref var transformComponent = ref _transformPool.Get(killedEntity);
                    transform = transformComponent.Value;
                }
                
                _world.DelEntity(killedEntity);

                if(transform !=null && transform.gameObject!=null)
                    transform.gameObject.Despawn();
            }
        }
    }
}