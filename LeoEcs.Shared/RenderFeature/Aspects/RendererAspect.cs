namespace UniGame.LeoEcs.Shared.Components
{
    using System;
    using Leopotam.EcsLite;
    using Bootstrap.Runtime.Abstract;

    [Serializable]
    public class RendererAspect : EcsAspect
    {
        public EcsPool<RendererComponent> Render;
        public EcsPool<RendererEnabledComponent> Enabled;
        public EcsPool<RendererVisibleComponent> Visible;
    }
}