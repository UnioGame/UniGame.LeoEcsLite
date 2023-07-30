namespace Game.Ecs.Characteristics.Health.Components
{
    using System;
    using Unity.IL2CPP.CompilerServices;

    /// <summary>
    /// pause death process and await actions
    /// </summary>
    [Serializable]
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public struct AwaitDeathCompleteComponent
    {
        
    }
}