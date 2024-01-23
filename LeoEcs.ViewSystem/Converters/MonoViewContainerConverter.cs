namespace UniGame.LeoEcs.ViewSystem.Converters
{
    using System;
    using System.Threading;
    using Components;
    using Leopotam.EcsLite;
    using Shared.Extensions;
    using UiSystem.Runtime.Settings;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Converter.Runtime.Converters;
    using UnityEngine;

    public sealed class MonoViewContainerConverter : MonoLeoEcsConverter<ViewContainerConverter>
    {

    }

    [Serializable]
    public class ViewContainerConverter : LeoEcsConverter
    {
        public ViewId TargetView;
        
        public sealed override void Apply(GameObject target, EcsWorld world, int entity)
        {
            ref var viewContainer = ref world.GetOrAddComponent<ViewContainerComponent>(entity);
            viewContainer.ViewId = TargetView;
        }
    }
}