namespace UniGame.LeoEcsLite.LeoEcs.Shared.Components
{
    using System;

    /// <summary>
    /// hash value component
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct HashComponent
    {
        public int Value;
    }
}