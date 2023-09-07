namespace UniGame.LeoEcs.Shared.Components
{
    using System;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// Component wot GameObject info 
    /// </summary>
    [Serializable]
    public struct GameObjectComponent
    {
        [FormerlySerializedAs("GameObject")] public GameObject Value;
    }
}