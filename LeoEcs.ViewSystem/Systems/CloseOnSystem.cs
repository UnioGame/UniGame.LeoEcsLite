namespace Game.Ecs.UI.EndGameScreens.Systems
{
    using System;
    using Leopotam.EcsLite;
    using Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.ViewSystem.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using UniGame.LeoEcs.ViewSystem.Components;
    using UniGame.LeoEcs.ViewSystem.Extensions;
    using UniGame.ViewSystem.Runtime;

    /// <summary>
    /// request to show view in container
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class CloseOnSystem<TEvent,TViewModel> : IEcsInitSystem, IEcsRunSystem
        where TEvent : struct
        where TViewModel : IViewModel
    {
        private EcsWorld _world;
        private EcsFilter _eventFilter;
        private EcsFilter _viewFilter;

        private EcsPool<ViewComponent> _viewPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _eventFilter = _world.Filter<TEvent>().End();
            _viewFilter = _world
                .ViewFilter<TViewModel>()
                .Inc<ViewComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in _eventFilter)
            {
                foreach (var viewEntity in _viewFilter)
                {
                    ref var viewComponent = ref _viewPool.Get(viewEntity);
                    viewComponent.View.Close();
                }
            }
        }
    }
}