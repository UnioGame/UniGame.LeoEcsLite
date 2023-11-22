namespace UniGame.LeoEcs.Shared.Components
{
    using System;
    using Unity.Mathematics;

    /// <summary>
    /// Component with single transform data 
    /// </summary>
    [Serializable]
    public struct TransformScaleComponent
    {
        public float3 Scale;
        public float3 LocalScale;
    }
}