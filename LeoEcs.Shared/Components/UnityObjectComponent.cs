﻿namespace UniGame.LeoEcsLite.LeoEcs.Shared.Components
{
    using System;
    using Leopotam.EcsLite;
    using Object = UnityEngine.Object;

    /// <summary>
    /// reference to unity object
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct UnityObjectComponent : IEcsAutoReset<UnityObjectComponent>
    {
        public Object Value;
        
        public void AutoReset(ref UnityObjectComponent c)
        {
            c.Value = null;
        }
    }
}