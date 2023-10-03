namespace UniGame.LeoEcs.ViewSystem.Layouts.Components
{
    using System;

    /// <summary>
    /// mark layout for registering into view system
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct RegisterViewLayoutSelfRequest
    {
        
    }
}