namespace Game.Ecs.Core.Death.Components
{
    using System;
    using Leopotam.EcsLite;
    using Unity.IL2CPP.CompilerServices;

    /// <summary>
    /// destroy target entity without pooling
    /// </summary>
    [Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct DestroySelfRequest : IEcsAutoReset<DestroySelfRequest>
    {
        public bool ForceDestroy;
        
        public void AutoReset(ref DestroySelfRequest c)
        {
            c.ForceDestroy = false;
        }
    }
}