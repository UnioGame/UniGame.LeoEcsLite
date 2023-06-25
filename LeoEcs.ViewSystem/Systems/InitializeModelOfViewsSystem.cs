namespace UniGame.LeoEcs.ViewSystem.Systems
{
    using System;
    using Behavriour;
    using Components;
    using Leopotam.EcsLite;
    using Shared.Extensions;
    using Unity.IL2CPP.CompilerServices;

#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class InitializeModelOfViewsSystem : IEcsInitSystem,IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _filter;
        
        private EcsPool<ViewComponent> _viewComponentPool;
        private EcsPool<ViewInitializedComponent> _initializedPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<ViewComponent>()
                .Exc<ViewInitializedComponent>()
                .End();

            _viewComponentPool = _world.GetPool<ViewComponent>();
            _initializedPool = _world.GetPool<ViewInitializedComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var viewComponent = ref _viewComponentPool.Get(entity);
                var view = viewComponent.View;

                if(view.ViewModel == null) continue;

                ref var viewModelComponent = ref _world.GetOrAddComponent<ViewModelComponent>(entity);
                viewModelComponent.Model = view.ViewModel;

                _initializedPool.GetOrAddComponent(entity);
            }
        }

    }
}