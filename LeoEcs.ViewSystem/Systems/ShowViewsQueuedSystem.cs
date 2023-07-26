namespace UniGame.LeoEcsLite.LeoEcs.ViewSystem.Systems
{
    using System;
    using Game.Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.ViewSystem.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.ViewSystem.Components;

    /// <summary>
    /// show view queued one by one
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class ShowViewsQueuedSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _viewFilter;
        private EcsFilter _viewQueuedFilter;
        
        private EcsPool<ShowQueuedRequest> _queuedPool;
        private EcsPool<ViewIdComponent> _viewIdPool;
        private IEcsPool _viewRequestPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _viewFilter = _world
                .Filter<ViewIdComponent>()
                .End();
            
            _viewQueuedFilter = _world
                .Filter<ShowQueuedRequest>()
                .End();

            _viewRequestPool = _world.GetPoolByType(typeof(CreateViewRequest));
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var requestEntity in _viewQueuedFilter)
            {
                ref var queuedRequest = ref _queuedPool.Get(requestEntity);
                
                if (queuedRequest.Value.Count == 0)
                {
                    _queuedPool.Del(requestEntity);
                    continue;
                }
                
                var activateNext = queuedRequest.AwaitId == 0;
                
                foreach (var viewEntity in _viewFilter)
                {
                    if(activateNext) break;
                    ref var viewIdComponent = ref _viewIdPool.Get(viewEntity);
                    activateNext = viewIdComponent.Value == queuedRequest.AwaitId;
                }
                
                if(!activateNext) continue;
                
                var viewRequestEntity = _world.NewEntity();
                var nextRequest = queuedRequest.Value.Dequeue();
                queuedRequest.AwaitId = nextRequest.ViewId.GetHashCode();
                _viewRequestPool.AddRaw(viewRequestEntity, nextRequest);
            }
            
        }
    }
}