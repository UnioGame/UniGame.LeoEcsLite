namespace Game.Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.ViewSystem.Components
{
    using System;

    /// <summary>
    /// id of view with it was created
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct ViewIdComponent
    {
        public int Value;
    }
}