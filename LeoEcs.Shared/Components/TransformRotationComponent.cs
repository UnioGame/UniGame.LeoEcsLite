namespace UniGame.LeoEcs.Shared.Components
{
    using System;
    using Unity.Mathematics;
    using UnityEngine;

    /// <summary>
    /// rotation data
    /// </summary>
    [Serializable]
    public struct TransformRotationComponent
    {
        public float3 Euler;
        public Quaternion Quaternion;
        public Quaternion LocalRotation;
    }
}