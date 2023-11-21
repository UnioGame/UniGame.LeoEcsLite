namespace UniGame.LeoEcs.ViewSystem.Layouts.Aspects
{
    using Components;
    using Game.Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.ViewSystem.Components;
    using Bootstrap.Runtime.Abstract;
    using Leopotam.EcsLite;

    public class ViewLayoutAspect : EcsAspect
    {
        public EcsPool<ViewLayoutComponent> Layout;
        public EcsPool<ViewParentComponent> Parent;
        
        //operations
        public EcsPool<RegisterViewLayoutSelfRequest> Register;
        public EcsPool<RemoveViewLayoutSelfRequest> Remove;
    }
}