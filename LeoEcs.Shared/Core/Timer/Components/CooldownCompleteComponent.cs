namespace UniGame.LeoEcs.Timer.Components
{
    using System;

    /// <summary>
    /// marks cooldown as passed
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct CooldownCompleteComponent
    {
        
    }
}