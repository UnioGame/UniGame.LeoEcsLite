﻿namespace UniGame.LeoEcs.ViewSystem.Components
{
    using System;
    using Unity.IL2CPP.CompilerServices;

    /// <summary>
    /// is view request already created for entity
    /// </summary>
    [Serializable]
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public struct SingleViewMarkerComponent<TView>
    {
        
    }
}