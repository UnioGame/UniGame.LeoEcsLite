namespace Game.Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.ViewSystem.Systems
{
    using System;
    using Components;
    using global::UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using global::UniModules.UniGame.UISystem.Runtime;
    using Leopotam.EcsLite;

    /// <summary>
    /// close View if entity is dead
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class CloseViewByDeadEntitySystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _filter;

        private EcsPool<ViewEntityLifeTimeComponent> _lifeTimePool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _filter = _world
                .Filter<ViewEntityLifeTimeComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var lifeTimeComponent = ref _lifeTimePool.Get(entity);
                if(lifeTimeComponent.Value.Unpack(_world,out _))
                    continue;
                
                var view = lifeTimeComponent.View;
                if(!view.IsTerminated && view.Status.Value != ViewStatus.Closed)
                    view.Close();
    
                _lifeTimePool.Del(entity);
            }
        }
    }
}