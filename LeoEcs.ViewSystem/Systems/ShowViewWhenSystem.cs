namespace Game.Ecs.UI.EndGameScreens.Systems
{
    using System;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using UniGame.LeoEcs.ViewSystem.Components;
    using UniGame.ViewSystem.Runtime;
    
    /// <summary>
    /// await target event and create view
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class ShowViewWhenSystem<TView> : IEcsInitSystem, IEcsRunSystem
        where TView : IView
    {
        private ViewRequestData _data;
        private EcsWorld _world;
        private EcsFilter _eventFilter;

        public ShowViewWhenSystem(EcsFilter eventFilter,ViewRequestData data)
        {
            _eventFilter = eventFilter;
            _data = data;
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
                ref var requestComponent = ref _world.AddComponent<CreateViewRequest>(requestEntity);

                var parent = _data.Parent;
                if (parent != null)
                {
                    var ecsConverter = parent.gameObject.GetComponent<LeoEcsMonoConverter>();
                    if (ecsConverter != null && ecsConverter.IsPlayingAndReady)
                        requestComponent.Owner = ecsConverter.PackedEntity;
                }

                requestComponent.Parent = parent;
                requestComponent.ViewId = typeof(TView).Name;
                requestComponent.LayoutType = requestComponent.LayoutType;
                requestComponent.Tag = requestComponent.Tag;
                requestComponent.ViewName = requestComponent.ViewName;
                requestComponent.StayWorld = requestComponent.StayWorld;
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
    public class ShowViewWhenSystem<TEvent,TView> : IEcsInitSystem, IEcsRunSystem
        where TEvent : struct
        where TView : IView
    {
        private ViewRequestData _data;
        private EcsWorld _world;
        private EcsFilter _eventFilter;

        public ShowViewWhenSystem(ViewRequestData data)
        {
            _data = data;
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
                ref var requestComponent = ref _world.AddComponent<CreateViewRequest>(requestEntity);

                var parent = _data.Parent;
                if (parent != null)
                {
                    var ecsConverter = parent.gameObject.GetComponent<LeoEcsMonoConverter>();
                    if (ecsConverter != null && ecsConverter.IsPlayingAndReady)
                        requestComponent.Owner = ecsConverter.PackedEntity;
                }

                requestComponent.Parent = parent;
                requestComponent.ViewId = typeof(TView).Name;
                requestComponent.LayoutType = requestComponent.LayoutType;
                requestComponent.Tag = requestComponent.Tag;
                requestComponent.ViewName = requestComponent.ViewName;
                requestComponent.StayWorld = requestComponent.StayWorld;
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
    public class ShowViewWhenSystem<TEvent1,TEvent2,TView> : IEcsInitSystem, IEcsRunSystem
        where TEvent1 : struct
        where TEvent2 : struct
        where TView : IView
    {
        private ViewRequestData _data;
        private EcsWorld _world;
        private EcsFilter _eventFilter;

        public ShowViewWhenSystem(ViewRequestData data)
        {
            _data = data;
        }
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _eventFilter = _world.Filter<TEvent1>()
                .Inc<TEvent2>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in _eventFilter)
            {
                var requestEntity = _world.NewEntity();
                ref var requestComponent = ref _world.AddComponent<CreateViewRequest>(requestEntity);

                var parent = _data.Parent;
                if (parent != null)
                {
                    var ecsConverter = parent.gameObject.GetComponent<LeoEcsMonoConverter>();
                    if (ecsConverter != null && ecsConverter.IsPlayingAndReady)
                        requestComponent.Owner = ecsConverter.PackedEntity;
                }

                requestComponent.Parent = parent;
                requestComponent.ViewId = typeof(TView).Name;
                requestComponent.LayoutType = requestComponent.LayoutType;
                requestComponent.Tag = requestComponent.Tag;
                requestComponent.ViewName = requestComponent.ViewName;
                requestComponent.StayWorld = requestComponent.StayWorld;
            }
        }
    }
}