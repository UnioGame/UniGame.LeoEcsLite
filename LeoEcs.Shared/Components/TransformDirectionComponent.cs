namespace UniGame.LeoEcs.Shared.Components
{
    using System;
    using Unity.Mathematics;

    /// <summary>
    /// Component with single transform data 
    /// </summary>
    [Serializable]
    public struct TransformDirectionComponent
    {
        public float3 Forward;
        public float3 Right;
        public float3 Up;
    }
}