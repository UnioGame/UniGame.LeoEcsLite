namespace Game.Ecs.Core.Converters
{
    using System;
    using System.Threading;
    using Leopotam.EcsLite;
    using Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.Shared.Components;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using Unity.IL2CPP.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// renderer converter
    /// </summary>
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [Serializable]
    public class RenderConverter : GameObjectConverter
    {
        protected override void OnApply(GameObject target,
            EcsWorld world, 
            int entity,
            CancellationToken cancellationToken = default)
        {
            var render = target.GetComponent<Renderer>();
            if(render == null) return;

            ref var renderComponent = ref world.GetOrAddComponent<RenderComponent>(entity);
            ref var visibleComponent = ref world.GetOrAddComponent<VisibleRenderComponent>(entity);

            renderComponent.Value = render;
            visibleComponent.Value = render.isVisible;

            if (render.enabled)
            {
                ref var activeComponent = ref world.GetOrAddComponent<RenderEnabledComponent>(entity);
            }
        }

        
    }
}