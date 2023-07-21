namespace UniGame.LeoEcs.ViewSystem.Components
{
    using System;
    using Leopotam.EcsLite;

    [Serializable]
    public class ViewContainerSystemData
    {
        public string View = string.Empty;
        public bool UseBusyContainer = false;
        public EcsPackedEntity Owner = default;
        public bool OwnViewBySource = false;
        public bool Single = false;
        public string Tag = string.Empty;
        public string ViewName = string.Empty;
        public bool StayWorld = false;
        public EcsWorld.Mask FilterMask = null;
    }
}