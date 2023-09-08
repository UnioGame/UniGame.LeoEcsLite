using System;
using Leopotam.EcsLite;

namespace UniGame.LeoEcs.Shared.Components
{
    using UnityEngine.Serialization;

    /// <summary>
    /// link to owner entity, but don't destroy on owner destroy
    /// </summary>
    [Serializable]
    public struct OwnerLinkComponent
    {
        public EcsPackedEntity Entity;
    }
    
    [Serializable]
    public struct LinkComponent<T>
    {
        [FormerlySerializedAs("Entity")] public EcsPackedEntity Value;
    }
    
    [Serializable]
    public struct LinkComponent
    {
        public EcsPackedEntity Value;
    }
    
    [Serializable]
    public struct ValueLinkComponent<T>
    {
        public T Value;
    }
}