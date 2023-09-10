namespace Game.Ecs.Core.Aspects
{
    using System;
    using Leopotam.EcsLite;
    using Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.Shared.Components;
    using UniGame.LeoEcsLite.LeoEcs.Bootstrap.Runtime.Abstract;

    [Serializable]
    public class RendererAspect : EcsAspect
    {
        public EcsPool<RenderComponent> Render;
        public EcsPool<RenderEnabledComponent> Enabled;
        public EcsPool<VisibleRenderComponent> Visible;
    }
}