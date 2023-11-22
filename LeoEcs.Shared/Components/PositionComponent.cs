namespace UniGame.LeoEcsLite.LeoEcs.Shared.Components
{
    using System;
    using Leopotam.EcsLite;
    using Unity.Mathematics;
    using UnityEngine;

    /// <summary>
    /// point in the world space
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct PositionComponent : IEcsAutoReset<PositionComponent>
    {
        public float3 Value;
        
        public void AutoReset(ref PositionComponent c)
        {
            c.Value = float3.zero;
        }
    }
}