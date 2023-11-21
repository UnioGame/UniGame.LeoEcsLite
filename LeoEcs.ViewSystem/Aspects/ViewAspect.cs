namespace UniGame.LeoEcs.ViewSystem.Aspects
{
    using System;
    using Components;
    using Converter.Runtime.Components;
    using Bootstrap.Runtime.Abstract;
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
    }
}