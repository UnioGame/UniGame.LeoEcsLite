namespace UniGame.LeoEcs.Shared.Components
{
    using System;
    using UnityEngine.Serialization;

    [Serializable]
    public struct NameComponent
    {
        [FormerlySerializedAs("Name")] public string Value;
    }
}