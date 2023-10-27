namespace Game.Ecs.Core.Death.Systems
{
    using System;
    using Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Components;
    using UniGame.LeoEcs.Shared.Extensions;
    using UniGame.Runtime.ObjectPool.Extensions;
    using UnityEngine;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public sealed class ProcessDespawnSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsWorld _world;
        
        private EcsFilter _filter;
        private EcsFilter _eventFilter;
        

        private EcsPool<DeadEvent> _deadEventPool;
        private EcsPool<TransformComponent> _transformPool;
        private EcsPool<PoolingComponent> _pooledPool;
        private EcsPool<DontKillComponent> _dontKillPool;
        private EcsPool<GameObjectComponent> _gameObjectPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _eventFilter = _world
                .Filter<KillEvent>()
                .End();
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
                var isGameObject = _gameObjectPool.Has(killedEntity);
                
                GameObject gameObject = null;

                if (isGameObject)
                {
                    ref var gameObjectComponent = ref _gameObjectPool.Get(killedEntity);
                    gameObject = gameObjectComponent.Value;
                }
                else if (isTransform)
                {
                    ref var transformComponent = ref _transformPool.Get(killedEntity);
                    var transform = transformComponent.Value;
                    gameObject = transform?.gameObject;
                }
                
                _world.DelEntity(killedEntity);

                if(gameObject != null)
                    gameObject.Despawn();
            }
        }
    }
}