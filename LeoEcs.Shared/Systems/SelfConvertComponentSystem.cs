namespace Game.Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.Shared.Systems
{
    using System;
    using Leopotam.EcsLite;
    
    /// <summary>
    /// create target component on entity by triggered component
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class SelfConvertComponentSystem<TTrigger,TTarget> : IEcsInitSystem, IEcsRunSystem
        where TTrigger : struct
        where TTarget : struct
    {
        private EcsWorld _world;
        private EcsFilter _filter;
        private EcsPool<TTarget> _targetPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _filter = _world
                .Filter<TTrigger>()
                .Exc<TTarget>()
                .End();

            _targetPool = _world.GetPool<TTarget>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                _targetPool.Add(entity);
            }
        }
    }
}