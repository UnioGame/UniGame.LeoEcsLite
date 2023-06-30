namespace Game.Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.Shared.Components
{
    using System;
    using UnityEngine;

    /// <summary>
    /// unity light component
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct LightComponent
    {
        public Light Value;
    }
}