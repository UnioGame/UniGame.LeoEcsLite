namespace UniGame.LeoEcs.Shared.Components
{
    using System;
    using Unity.Mathematics;
    using UnityEngine.Serialization;

    /// <summary>
    /// Component with single transform data 
    /// </summary>
    [Serializable]
    public struct TransformPositionComponent
    {
        public float3 Position;
    }
}