namespace UniGame.LeoEcsLite.LeoEcs.ViewSystem.Systems
{
    using System;
    using global::UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using global::UniGame.LeoEcs.ViewSystem.Components;
    using global::UniGame.LeoEcs.ViewSystem.Extensions;
    using Leopotam.EcsLite;

    /// <summary>
    /// make request to shown view queued
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class ShowQueuedViewOnSystem<TEvent,TView1,TView2> : IEcsInitSystem, IEcsRunSystem where TEvent : struct
    {
        private readonly EcsViewData _viewData;
        private EcsWorld _world;
        
        private EcsPool<ShowQueuedRequest> _showRequestPool;
        private EcsFilter _filter;

        public ShowQueuedViewOnSystem(EcsViewData viewData)
        {
            _viewData = viewData;
        }
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<TEvent>()
                .End();
            
            _showRequestPool = _world.GetPool<ShowQueuedRequest>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                var requestEntity = _world.NewEntity();
                ref var request = ref _showRequestPool.Add(requestEntity);
                
                request.AwaitId = 0;
                
                request.Value.Enqueue(EcsViewExtensions.CreateViewRequest(typeof(TView1).Name,
                    _viewData.LayoutType,_viewData.Parent,_viewData.Tag,_viewData.ViewName,_viewData.StayWorld));
                request.Value.Enqueue(EcsViewExtensions.CreateViewRequest(typeof(TView2).Name,
                    _viewData.LayoutType,_viewData.Parent,_viewData.Tag,_viewData.ViewName,_viewData.StayWorld));
            }
        }
    }
}