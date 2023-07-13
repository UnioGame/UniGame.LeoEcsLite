namespace UniGame.LeoEcs.ViewSystem.Systems
{
    using System;
    using Components;
    using Game.Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.ViewSystem.Components;
    using global::UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using Leopotam.EcsLite;
    using Shared.Components;

    /// <summary>
    /// check is container state is changed to free
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class UpdateViewContainerBusyStatusSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _containerFilter;
        private EcsFilter _parentingViewFilter;

        private EcsPool<TransformComponent> _transformPool;
        private EcsPool<ViewParentComponent> _parentPool;
        private EcsPool<ViewContainerBusyComponent> _busyPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _containerFilter =_world
                .Filter<ViewContainerComponent>()
                .Inc<TransformComponent>()
                .Inc<ViewContainerBusyComponent>()
                .End();

            _parentingViewFilter = _world
                .Filter<ViewParentComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var containerEntity in _containerFilter)
            {
                ref var transformComponent = ref _transformPool.Get(containerEntity);
                var isEmpty = true;
                
                foreach (var parentEntity in _parentingViewFilter)
                {
                    ref var parentComponent = ref _parentPool.Get(parentEntity);
                    if (parentComponent.Value != transformComponent.Value) continue;
                    
                    isEmpty = false;
                    break;
                }
                
                if(isEmpty) _busyPool.Del(containerEntity);
            }
        }
    }
}