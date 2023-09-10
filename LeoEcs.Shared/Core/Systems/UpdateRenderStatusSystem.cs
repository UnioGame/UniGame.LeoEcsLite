namespace Game.Ecs.Core.Systems
{
    using System;
    using System.Linq;
    using Aspects;
    using Leopotam.EcsLite;
    using Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.Shared.Components;
    using UniGame.Core.Runtime.Extension;
    using UniGame.LeoEcs.Shared.Extensions;
    using UniGame.Runtime.ObjectPool.Extensions;
    using UnityEngine;
    using UnityEngine.Pool;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

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
    [ECSDI]
    public class UpdateRenderStatusSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _filter;
        private RendererAspect _render;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world.Filter<RenderComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var renderComponent = ref _render.Render.Get(entity);

                var render = renderComponent.Value;
                if (render.enabled)
                {
                    _render.Enabled.GetOrAddComponent(entity);
                }
                else
                {
                    _render.Enabled.TryRemove(entity);
                }
                
                if (render.isVisible)
                {
                    _render.Visible.GetOrAddComponent(entity);
                }
                else
                {
                    _render.Visible.TryRemove(entity);
                }
            }
        }
    }
}