namespace UniGame.LeoEcs.ViewSystem.Layouts.Components
{
    using System;
    using UniGame.ViewSystem.Runtime;

    /// <summary>
    /// view layout data
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct ViewLayoutComponent
    {
        public string Id;
        public IViewLayout Layout;
    }
}