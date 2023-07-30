namespace Game.Ecs.Core.Components
{
    using UnityEngine;

    public struct GroundInfoComponent
    {
        public float CheckDistance;
        
        public Vector3 Normal;
        public bool IsGrounded;
    }
}