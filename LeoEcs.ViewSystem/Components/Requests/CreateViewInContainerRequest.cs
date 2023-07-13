namespace UniGame.LeoEcs.ViewSystem.Components
{
    using System;
    using Leopotam.EcsLite;
    using UnityEngine.Serialization;

    /// <summary>
    /// ask to create view in container
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct CreateViewInContainerRequest
    {
        [FormerlySerializedAs("ViewId")] public string View;

        /// <summary>
        /// if true then use busy container, else wait for free one
        /// </summary>
        public bool UseBusyContainer;
        
        /// <summary>
        /// Optional Data
        /// </summary>
        public string Tag;
        
        /// <summary>
        /// Optional Data
        /// </summary>
        public string ViewName;
        
        /// <summary>
        /// Optional Data
        /// </summary>
        public bool StayWorld;
        
        /// <summary>
        /// Optional Data
        /// </summary>
        public EcsPackedEntity Owner;
    }
}