namespace UniGame.LeoEcsLite.LeoEcs.ViewSystem.Systems
{
    using System;
    using Game.Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.ViewSystem.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.ViewSystem.Components;

    /// <summary>
    /// initialize viewid component for view entity
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class InitializeViewIdComponentSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _filter;
        
        private EcsPool<ViewComponent> _viewComponentPool;
        private EcsPool<ViewIdComponent> _viewIdPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<ViewComponent>()
                .Inc<ViewInitializedComponent>()
                .Exc<ViewIdComponent>()
                .End();

            _viewComponentPool = _world.GetPool<ViewComponent>();
            _viewIdPool = _world.GetPool<ViewIdComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var viewEntity in _filter)
            {
                ref var viewComponent = ref _viewComponentPool.Get(viewEntity);
                var view = viewComponent.View;
                ref var viewIdComponent = ref _viewIdPool.Add(viewEntity);
                viewIdComponent.Value = view.ViewIdHash;
            }
        }
    }
}