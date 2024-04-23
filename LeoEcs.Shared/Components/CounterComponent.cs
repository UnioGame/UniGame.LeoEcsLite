using System;
using Leopotam.EcsLite;

namespace UniGame.LeoEcs.Shared.Components
{
    /// <summary>
    /// request component that will be deleted after "counter" cycles
    /// </summary>
    [Serializable]
    public struct CounterComponent<T> : IEcsAutoReset<CounterComponent<T>>
    {
        public int counter;
        
        public void AutoReset(ref CounterComponent<T> c)
        {
            c.counter = 0;
        }
    }
}