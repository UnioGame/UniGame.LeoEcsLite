﻿namespace Game.Ecs.UI.EndGameScreens.Systems
{
    using System;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Shared.Extensions;
    using UniGame.LeoEcs.ViewSystem.Components;
    using UniGame.ViewSystem.Runtime;
    using UniModules.UniGame.UISystem.Runtime.WindowStackControllers.Abstract;
    using Unity.IL2CPP.CompilerServices;
    using ViewType = UniModules.UniGame.UiSystem.Runtime.ViewType;
    
    /// <summary>
    /// await target event and create view
    /// </summary>
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class ShowSingleLayoutViewWhen<TEvent,TView> : IEcsInitSystem, IEcsRunSystem
        where TEvent : struct
        where TView : IView
    {
        private ViewType _viewLayoutType;
        
        private EcsWorld _world;
        private EcsFilter _eventFilter;

        public ShowSingleLayoutViewWhen(ViewType viewLayoutType = ViewType.Window)
        {
            _viewLayoutType = viewLayoutType;
        }
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _eventFilter = _world.Filter<TEvent>()
                .Exc<SingleViewMarkerComponent<TView>>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in _eventFilter)
            {
                var requestEntity = _world.NewEntity();
                ref var requestComponent = ref _world.AddComponent<CreateLayoutViewRequest>(requestEntity);
                ref var markerComponent = ref _world.AddComponent<SingleViewMarkerComponent<TView>>(eventEntity);

                requestComponent.View = typeof(TView).Name;
                requestComponent.LayoutType = _viewLayoutType;
            }
        }
    }
            
    /// <summary>
    /// await target event and create view
    /// </summary>
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class ShowLayoutViewWhen<TEvent,TView> : IEcsInitSystem, IEcsRunSystem
        where TEvent : struct
        where TView : IView
    {
        private ViewType _viewLayoutType;
        
        private EcsWorld _world;
        private EcsFilter _eventFilter;

        public ShowLayoutViewWhen(ViewType viewLayoutType = ViewType.Window)
        {
            _viewLayoutType = viewLayoutType;
        }
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _eventFilter = _world.Filter<TEvent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in _eventFilter)
            {
                var requestEntity = _world.NewEntity();
                ref var requestComponent = ref _world.AddComponent<CreateLayoutViewRequest>(requestEntity);

                requestComponent.View = typeof(TView).Name;
                requestComponent.LayoutType = _viewLayoutType;
            }
        }
    }
    
    /// <summary>
    /// await target event and create view
    /// </summary>
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class ShowLayoutViewWhen<TView> : IEcsInitSystem, IEcsRunSystem
        where TView : IView
    {
        private ViewType _viewLayoutType;
        
        private EcsWorld _world;
        private EcsFilter _eventFilter;

        public ShowLayoutViewWhen(EcsFilter eventFilter,ViewType viewLayoutType = ViewType.Window)
        {
            _eventFilter = eventFilter;
            _viewLayoutType = viewLayoutType;
        }
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in _eventFilter)
            {
                var requestEntity = _world.NewEntity();
                ref var requestComponent = ref _world.AddComponent<CreateLayoutViewRequest>(requestEntity);

                requestComponent.View = typeof(TView).Name;
                requestComponent.LayoutType = _viewLayoutType;
            }
        }
    }
    
    /// <summary>
    /// await target event and create view
    /// </summary>
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class ShowLayoutViewWhen<TEvent1,TEvent2,TView> : IEcsInitSystem, IEcsRunSystem
        where TEvent1 : struct
        where TEvent2 : struct
        where TView : IView
    {
        private ViewType _viewLayoutType;
        
        private EcsWorld _world;
        private EcsFilter _eventFilter;

        public ShowLayoutViewWhen(ViewType viewLayoutType = ViewType.Window)
        {
            _viewLayoutType = viewLayoutType;
        }
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _eventFilter = _world
                .Filter<TEvent1>()
                .Inc<TEvent2>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in _eventFilter)
            {
                var requestEntity = _world.NewEntity();
                ref var requestComponent = ref _world.AddComponent<CreateLayoutViewRequest>(requestEntity);

                requestComponent.View = typeof(TView).Name;
                requestComponent.LayoutType = _viewLayoutType;
            }
        }
    }
}