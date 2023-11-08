namespace UniGame.LeoEcs.ViewSystem.Converters
{
    using System;
    using UnityEngine;

    [Serializable]
    public class EcsViewSettings
    {
        [Tooltip("if true - view will be closed when entity is destroyed")]
        public bool followEntityLifeTime = false;
        [Tooltip("if true - add view order component to entity")]
        public bool addChildOrderComponent = false;
        [Tooltip("if true - add update view request on convert")]
        public bool addUpdateRequestOnCreate = true;
    }
}