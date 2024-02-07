namespace UniGame.LeoEcsLite.LeoEcs.Shared.Components
{
    using System;
    using Leopotam.EcsLite;
    using UniModules.UniCore.Runtime.DataFlow;

    /// <summary>
    /// lifetime component
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct LifeTimeComponent : IEcsAutoReset<LifeTimeComponent>
    {
        public LifeTime Value;
        
        public void AutoReset(ref LifeTimeComponent c)
        {
            c.Value ??= LifeTime.Create();
            c.Value.Release();
        }
    }
}