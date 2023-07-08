namespace Game.Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.ViewSystem.Components
{
    using System;
    using global::UniGame.ViewSystem.Runtime;
    using Leopotam.EcsLite;

    /// <summary>
    /// link view with entity and follow it's lifetime
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct ViewEntityLifeTimeComponent
    {
        public EcsPackedEntity Value;
        public IView View;
    }
}