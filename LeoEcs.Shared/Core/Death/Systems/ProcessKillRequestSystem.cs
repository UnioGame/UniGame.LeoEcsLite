namespace Game.Ecs.Core.Death.Systems
{
    using System;
    using Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public sealed class ProcessKillRequestSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsWorld _world;

        private EcsPool<DestroyComponent> _deadPool;
        private EcsPool<PoolingComponent> _poolingPool;
        private EcsPool<DeadEvent> _deadEventPool;
        private EcsPool<KillRequest> _killRequestPool;
        private EcsPool<KillEvent> _killEventPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world
                .Filter<KillRequest>()
                .Exc<DestroyComponent>()
                .Exc<DontKillComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var killRequest = ref _killRequestPool.Get(entity);

                var killEventEntity = _world.NewEntity();
                ref var killEvent = ref _killEventPool.Add(killEventEntity);
                killEvent.Source = killRequest.Source;
                killEvent.Destination = _world.PackEntity(entity);

                if (_poolingPool.Has(entity))
                {
                    _deadEventPool.TryAdd(entity);
                    continue;
                }

                _deadPool.TryAdd(entity);
                _deadEventPool.TryAdd(entity);
            }
        }
    }
}