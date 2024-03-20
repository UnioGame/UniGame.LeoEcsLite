﻿namespace UniGame.LeoEcs.ViewSystem.Components
{
    using System;
    using Leopotam.EcsLite;

    /// <summary>
    /// link to view entity
    /// </summary>
    [Serializable]
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public struct ViewEntityDataComponent
    {
        public EcsPackedEntity Value;
    }
}