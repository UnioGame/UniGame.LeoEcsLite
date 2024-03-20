namespace Game.Ecs.Characteristics.Health.Components
{
    using System;

    /// <summary>
    /// pause death process and await actions
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct AwaitDeathCompleteComponent
    {
        
    }
}