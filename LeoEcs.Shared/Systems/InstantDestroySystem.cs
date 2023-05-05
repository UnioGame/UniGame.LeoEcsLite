namespace UniGame.LeoEcsLite.LeoEcs.Bootstrap.Runtime.Systems
{
    using System;
    using Components;
    using Leopotam.EcsLite;

    /// <summary>
    /// Add an empty target to an ability
    /// </summary>
    [Serializable]
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public class InstantDestroySystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;

        private EcsFilter _filter;

        public void Init(EcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world.Filter<InstantDestroyComponent>().End();
        }

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                _world.DelEntity(entity);
            }
        }
    }
}