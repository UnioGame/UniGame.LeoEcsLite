namespace UniGame.LeoEcsLite.LeoEcs.Bootstrap.Runtime.Components
{
    using System;

    /// <summary>
    /// mark entity as instant destroy
    /// </summary>
    [Serializable]
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public struct InstantDestroyComponent
    {
        
    }
}