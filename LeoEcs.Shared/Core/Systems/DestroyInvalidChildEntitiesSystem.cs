namespace Game.Ecs.Core.Systems
{
    using System;
    using Components;
    using Death.Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
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