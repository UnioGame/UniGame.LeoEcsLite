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
        
        public void Init(EcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<ViewComponent>()
                .Inc<ViewInitializedComponent>()
                .Exc<ViewModelComponent>()
                .End();

            _viewComponentPool = _world.GetPool<ViewComponent>();
        }

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var viewComponent = ref _viewComponentPool.Get(entity);
                var view = viewComponent.View;

                if (!view.IsInitialized.Value) continue;
                if(view.ViewModel == null) continue;

                ref var viewModelComponent = ref _world.GetOrAddComponent<ViewModelComponent>(entity);
                viewModelComponent.Model = view.ViewModel;
            }
        }

    }
}