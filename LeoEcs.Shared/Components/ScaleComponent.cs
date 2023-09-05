namespace Game.Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.Shared.Components
{
    using System;
    using Leopotam.EcsLite;
    using UnityEngine;

    /// <summary>
    /// scale component
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct ScaleComponent : IEcsAutoReset<ScaleComponent>
    {
        public Vector3 Value;
        
        public void AutoReset(ref ScaleComponent c)
        {
            c.Value = Vector3.one;
        }
    }
}