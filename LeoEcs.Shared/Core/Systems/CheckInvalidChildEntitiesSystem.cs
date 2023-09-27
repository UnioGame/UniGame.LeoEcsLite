namespace Game.Ecs.Core.Systems
{
    using System;
    using Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public sealed class CheckInvalidChildEntitiesSystem : IEcsRunSystem,IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsWorld _world;
        
        private EcsPool<OwnerComponent> _ownerPool;
        private EcsPool<OwnerDestroyedEvent> _ownerDestroyedPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world.Filter<OwnerComponent>().End();
        }
        
        public void Run(IEcsSystems systems)
        {
            
            foreach (var entity in _filter)
            {
                ref var ownerComponent = ref _ownerPool.Get(entity);
                if(ownerComponent.Value.Unpack(_world, out _))
                    continue;

                _ownerDestroyedPool.Add(entity);
            }
        }
    }
}