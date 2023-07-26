namespace UniGame.LeoEcs.ViewSystem.Components
{
    using System;
    using System.Collections.Generic;
    using Leopotam.EcsLite;

    /// <summary>
    /// request to show view one by one
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct ShowQueuedRequest : IEcsAutoReset<ShowQueuedRequest>
    {
        public int AwaitId;
        public Queue<CreateViewRequest> Value;
        
        public void AutoReset(ref ShowQueuedRequest c)
        {
            c.Value ??= new Queue<CreateViewRequest>();
            c.Value.Clear();
            c.AwaitId = 0;
        }
    }
}