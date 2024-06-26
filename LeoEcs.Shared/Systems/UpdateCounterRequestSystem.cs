﻿using System;
using Leopotam.EcsLite;
using UniGame.LeoEcs.Shared.Components;
using Unity.IL2CPP.CompilerServices;

namespace UniGame.LeoEcs.Shared.Systems
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [Serializable]
    public class UpdateCounterRequestSystem<TRequestType>: IEcsRunSystem,IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsWorld _world;
        private EcsPool<CounterComponent<TRequestType>> _pool;
        private int _counterLimit;
        
        public UpdateCounterRequestSystem(int counterLimit = 1)
        {
            _counterLimit = counterLimit;
        }
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world.Filter<CounterComponent<TRequestType>>().End();
            _pool = _world.GetPool<CounterComponent<TRequestType>>();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var component = ref _pool.Get(entity);
                if (component.counter < _counterLimit)
                {
                    component.counter += 1;
                    continue;
                }
                _pool.Del(entity);
            }
        }

    }
}