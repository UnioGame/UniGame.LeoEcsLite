namespace Game.Ecs.Core.Components
{
    using System;

    /// <summary>
    /// champion component
    /// </summary>
    [Serializable]
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public struct ChampionComponent
    {
        
    }
}