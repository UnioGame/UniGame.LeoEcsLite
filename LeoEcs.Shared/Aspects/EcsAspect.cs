namespace UniGame.LeoEcs.Bootstrap.Runtime.Abstract
{
    using System;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime;

    [Serializable]
    public abstract class EcsAspect : IEcsAspect
    {
        public virtual void Initialize(EcsWorld world)
        {
            
        }
    }
}