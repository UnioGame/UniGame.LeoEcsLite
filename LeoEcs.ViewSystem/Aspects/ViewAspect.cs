namespace UniGame.LeoEcs.ViewSystem.Aspects
{
    using System;
    using Components;
    using Converter.Runtime.Components;
    using Bootstrap.Runtime.Abstract;
    using Game.Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.ViewSystem.Components.Events;
    using Leopotam.EcsLite;
    using Shared.Components;
    using UnityEngine;

    [Serializable]
    public class ViewAspect : EcsAspect
    {
        public EcsPool<ViewComponent> View;
        public EcsPool<ViewModelComponent> Model;
        
        public EcsPool<ViewStatusComponent> Status;
        public EcsPool<ViewOrderComponent> Order;
        
        public EcsPool<TransformComponent> Transform;
        
        //events
        public EcsPool<ViewStatusSelfEvent> StatusChanged;
        
        //requests
        public EcsPool<CreateLayoutViewRequest> CreateLayoutView;
        public EcsPool<CloseViewSelfRequest> CloseView;
    }
}