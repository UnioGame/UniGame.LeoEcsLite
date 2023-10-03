using Leopotam.EcsLite;
using Transform = UnityEngine.Transform;

namespace UniGame.LeoEcs.ViewSystem.Components
{
    using System;
    using System.Runtime.CompilerServices;
    using Shared.Abstract;

    [Serializable]
    public struct CreateViewRequest : IEcsAutoReset<CreateViewRequest>, IApplyableComponent<CreateViewRequest>
    {
        public string ViewId;
        public string LayoutType;
        public string Tag;
        public Transform Parent;
        public string ViewName;
        public bool StayWorld;
        public EcsPackedEntity Owner;
        //target entity for view as a container
        public EcsPackedEntity Target;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AutoReset(ref CreateViewRequest c)
        {
            c.Tag = string.Empty;
            c.Parent = null;
            c.LayoutType = string.Empty;
            c.Target = default;
            c.Owner = default;
            c.ViewId = string.Empty;
            c.StayWorld = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Apply(ref CreateViewRequest component)
        {
            component.ViewId     = ViewId    ;
            component.LayoutType = LayoutType;
            component.Tag        = Tag       ;
            component.Parent     = Parent    ;
            component.ViewName   = ViewName  ;
            component.StayWorld  = StayWorld ;
            component.Owner      = Owner     ;
            component.Target     = Target    ;
        }
    }
}
