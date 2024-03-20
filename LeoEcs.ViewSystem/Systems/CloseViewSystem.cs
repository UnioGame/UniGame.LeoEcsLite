namespace UniGame.LeoEcs.ViewSystem.Systems
{
    using System;
    using Components;
    using Extensions;
    using Leopotam.EcsLite;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class CloseViewSystem : IEcsInitSystem,IEcsRunSystem
    {
        private EcsWorld _world;

        private EcsFilter _closeFilter;
        private EcsPool<ViewComponent> _viewComponent;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _closeFilter = _world
                .Filter<CloseViewSelfRequest>()
                .Inc<ViewComponent>()
                .End();
            
            _viewComponent = _world.GetPool<ViewComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _closeFilter)
            {
                ref var viewComponent = ref _viewComponent.Get(entity);
                viewComponent.View.Close();
                break;
            }
        }
    }
}