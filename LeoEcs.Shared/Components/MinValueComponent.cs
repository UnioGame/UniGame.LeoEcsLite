namespace Game.Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.Shared.Components
{
    using System;
    using UnityEngine.Serialization;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

    /// <summary>
    /// minimum value limitation
    /// </summary>
    [Serializable]
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public struct MinValueComponent
    {
        [FormerlySerializedAs("MinValue")] public float Value;
    }
}