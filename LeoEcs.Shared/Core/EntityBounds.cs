namespace Game.Ecs.Core
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct EntityBounds
    {
        public Vector3 Center;
        public float Height;
        public float Radius;
    }
}