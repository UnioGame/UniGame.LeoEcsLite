namespace UniGame.LeoEcs.ViewSystem.Layouts.Converters
{
    using System;
    using System.Threading;
    using Components;
    using Game.Ecs.Core.Components;
    using Game.Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.ViewSystem.Components;
    using Leopotam.EcsLite;
    using Shared.Extensions;
    using Sirenix.OdinInspector;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniModules.UniGame.Core.Runtime.DataFlow.Extensions;
    using Unity.IL2CPP.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// make a new layout for views
    /// </summary>
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [Serializable]
    public class EcsViewsLayoutConverter : GameObjectConverter
    {
        [InlineProperty]
        public ViewLayoutAsset layoutAsset;
        
        protected override void OnApply(
            GameObject target,
            EcsWorld world, 
            int entity)
        {
            var layoutId = layoutAsset.layoutId;
            var layoutFactory = layoutAsset.layout;

            var assetLifeTime = target.GetAssetLifeTime();
            var parent = target.transform;
            var layout = layoutFactory.Create(parent, null);

            ref var layoutComponent = ref world.GetOrAddComponent<ViewLayoutComponent>(entity);
            ref var registerComponent = ref world.GetOrAddComponent<RegisterViewLayoutSelfRequest>(entity);
            ref var parentComponent = ref world.GetOrAddComponent<ViewParentComponent>(entity);

            parentComponent.Value = parent;
            layoutComponent.Layout = layout;
            layoutComponent.Id = layoutId;
            
            //kill layout with asset
            assetLifeTime.AddDispose(layout);
        }
    }
}