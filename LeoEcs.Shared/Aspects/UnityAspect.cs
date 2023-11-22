namespace UniGame.LeoEcs.Bootstrap.Runtime.Abstract
{
    using System;
    using Leopotam.EcsLite;
    using Shared.Components;

    [Serializable]
    public class UnityAspect : EcsAspect
    {
        public EcsPool<GameObjectComponent> GameObject;
        public EcsPool<TransformComponent> Transform;
        public EcsPool<TransformPositionComponent> Position;
        public EcsPool<TransformDirectionComponent> Direction;
        public EcsPool<TransformScaleComponent> Scale;
        public EcsPool<TransformRotationComponent> Rotation;
    }
}