namespace UniGame.LeoEcs.ViewSystem.Converters
{
    using System;
    using System.Threading;
    using Components;
    using Converter.Runtime;
    using Converter.Runtime.Components;
    using Leopotam.EcsLite;
    using Shared.Extensions;
    using UnityEngine;

    [Serializable]
    public class ViewOrderConverter : GameObjectConverter
    {
        protected override void OnApply(GameObject target, EcsWorld world, int entity)
        {
            ref var dataComponent = ref world.GetOrAddComponent<ViewOrderComponent>(entity);
            dataComponent.Value = target.transform.GetSiblingIndex();
        }
    }
    
    [Serializable]
    public class ViewEntityDataConverter : GameObjectConverter
    {
        protected override void OnApply(GameObject target, EcsWorld world, int entity)
        {
            ref var dataComponent = ref world.GetOrAddComponent<ViewEntityDataComponent>(entity);
            dataComponent.Value = default;
        }
    }
}