namespace UniGame.LeoEcs.ViewSystem.Components
{
    using System;
    using UiSystem.Runtime.Settings;

    /// <summary>
    /// mark entity as a container for view
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct ViewContainerComponent
    {
        public ViewId ViewId;
    }
}