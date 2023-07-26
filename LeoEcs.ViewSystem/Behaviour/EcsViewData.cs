namespace UniGame.LeoEcs.ViewSystem.Components
{
    using System;
    using Leopotam.EcsLite;
    using UniModules.UniGame.UiSystem.Runtime;
    using UnityEngine;

    [Serializable]
    public class EcsViewData
    {
        public string View = string.Empty;
        public ViewType LayoutType;
        public EcsPackedEntity Owner = default;
        public Transform Parent;
        public bool Single = false;
        public string Tag = string.Empty;
        public string ViewName = string.Empty;
        public bool StayWorld = false;
        public EcsWorld.Mask FilterMask = null;
    }
}