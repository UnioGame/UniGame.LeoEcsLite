using Leopotam.EcsLite;
using UniModules.UniGame.UiSystem.Runtime;
using Transform = UnityEngine.Transform;

namespace UniGame.LeoEcs.ViewSystem.Components
{
    using System;
    using UnityEngine.Serialization;

    [Serializable]
    public struct CreateViewRequest : IEcsAutoReset<CreateViewRequest>
    {
        public string ViewId;
        public ViewType LayoutType;
        public string Tag;
        public Transform Parent;
        public string ViewName;
        public bool StayWorld;
        public EcsPackedEntity Owner;

        public void AutoReset(ref CreateViewRequest c)
        {
            c.Tag = string.Empty;
            c.Parent = null;
            c.LayoutType = ViewType.None;
        }
    }
}
