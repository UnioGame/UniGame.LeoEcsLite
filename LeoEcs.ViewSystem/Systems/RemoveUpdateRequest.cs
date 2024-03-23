﻿using System;
using Leopotam.EcsLite;
using UniGame.LeoEcs.ViewSystem.Components;
using UniGame.ViewSystem.Runtime;

namespace UniGame.LeoEcs.ViewSystem.Systems
{
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class RemoveUpdateRequest : IEcsRunSystem,IEcsInitSystem
    {
        private readonly IGameViewSystem _viewSystem;
        
        public EcsFilter _filter;
        public EcsWorld _world;

        public EcsPool<UpdateViewRequest> _updatePool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world.Filter<UpdateViewRequest>().End();
            
            _updatePool = _world.GetPool<UpdateViewRequest>();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var updateComponent = ref _updatePool.Get(entity);
                if (updateComponent.counter <= 0)
                {
                    updateComponent.counter += 1;
                    continue;
                } 
                _updatePool.Del(entity);
            }
        }

    }
}