namespace Game.Ecs.Core.Systems
{
    using System;
    using Leopotam.EcsLite;
    using Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.Shared.Components;
    using UniGame.LeoEcs.Shared.Extensions;

    /// <summary>
    /// update render components
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class UpdateRenderStatusSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _filter;
        private EcsPool<RenderComponent> _renderPool;
        private EcsPool<RenderEnabledComponent> _renderEnabledPool;
        private EcsPool<RenderVisibleComponent> _renderVisiblePool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world.Filter<RenderComponent>().End();

            _renderPool = _world.GetPool<RenderComponent>();
            _renderEnabledPool = _world.GetPool<RenderEnabledComponent>();
            _renderVisiblePool = _world.GetPool<RenderVisibleComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var renderComponent = ref _renderPool.Get(entity);

                var render = renderComponent.Value;
                if (render.enabled)
                {
                    _renderEnabledPool.GetOrAddComponent(entity);
                }
                else
                {
                    _renderEnabledPool.TryRemove(entity);
                }
                
                if (render.isVisible)
                {
                    _renderVisiblePool.GetOrAddComponent(entity);
                }
                else
                {
                    _renderVisiblePool.TryRemove(entity);
                }
            }
        }
    }
}