using System;
using Leopotam.EcsLite;
using UniGame.LeoEcs.Shared.Extensions;
using UniGame.LeoEcs.ViewSystem.Components;
using UniModules.UniGame.UiSystem.Runtime;
using UnityEngine;

namespace UniGame.LeoEcs.ViewSystem.Extensions
{
    using System.Runtime.CompilerServices;
    using UniGame.ViewSystem.Runtime;
    using UniModules.UniCore.Runtime.Utils;

    public static class EcsViewExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EcsWorld.Mask ViewFilter<TModel>(this EcsWorld world)
            where TModel : IViewModel
        {
            return world
                .Filter<ViewModelComponent>()
                .Inc<ViewDataComponent<TModel>>()
                .Inc<ViewInitializedComponent>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetViewModel<TModel>(this EcsWorld world, int entity, out TModel model)
        {
            model = default;
            if (!world.HasComponent<ViewModelComponent>(entity))
                return false;

            ref var viewModelComponent = ref world.GetComponent<ViewModelComponent>(entity);
            if (viewModelComponent.Model is not TModel viewModel) return false;

            model = viewModel;
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TModel GetViewModel<TModel>(this EcsWorld world, int entity) 
            where TModel : class
        {
            ref var viewModelComponent = ref world.GetComponent<ViewModelComponent>(entity);
            return viewModelComponent.Model as TModel;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EcsFilter CreateViewFilter<TModel>(this EcsWorld world)
            where TModel : IViewModel
        {
            return world.ViewFilter<TModel>().End();
        }
        
        public static int MakeViewRequest(
            this EcsWorld world,
            Type viewType,
            ViewType layoutType = ViewType.None,
            Transform parent = null,
            string tag = null,
            string viewName = null,
            bool stayWorld = false)
        {
            var target = new EcsPackedEntity();
            return MakeViewRequest(world, viewType.Name,ref target,ref target, layoutType, 
                parent, tag, viewName, stayWorld);
        }
        
        public static int MakeViewRequest(
            this EcsWorld world,
            Type viewType,
            ref  EcsPackedEntity targetEntity,
            ref EcsPackedEntity ownerEntity,
            ViewType layoutType = ViewType.None,
            Transform parent = null,
            string tag = null,
            string viewName = null,
            bool stayWorld = false)
        {
            return MakeViewRequest(world, viewType.Name,ref targetEntity,ref ownerEntity, layoutType, 
                parent, tag, viewName, stayWorld);
        }
        
        public static int MakeViewInContainerRequest<TView>(
            this EcsWorld world, 
            bool useBusyContainer = false,
            EcsPackedEntity owner = default,
            string tag = null,
            string viewName = null,
            bool stayWorld = false)
        {
            return world.MakeViewInContainerRequest(typeof(TView).Name, useBusyContainer, owner, tag, viewName, stayWorld);
        }
        
        public static int MakeViewInContainerRequest(
            this EcsWorld world, 
            string view,
            bool useBusyContainer = false,
            EcsPackedEntity owner = default,
            string tag = null,
            string viewName = null,
            bool stayWorld = false)
        {
            var entity = world.NewEntity();
            
            ref var component = ref world
                .GetOrAddComponent<CreateViewInContainerRequest>(entity);
            
            component.Tag = tag;
            component.View = view;
            component.ViewName = viewName;
            component.StayWorld = stayWorld;
            component.UseBusyContainer = useBusyContainer;
            component.Owner = owner;

            return entity;
        }
        
        public static int MakeViewRequest<TView>(
            this EcsWorld world, 
            ViewType layoutType)
        {
            return MakeViewRequest(world,typeof(TView),layoutType);
        }
        
        public static int MakeViewRequest(
            this EcsWorld world, 
            string viewId,
            ViewType layoutType)
        {
            return MakeViewRequest(world, viewId, layoutType.ToStringFromCache());
        }
        
        public static int MakeViewRequest(
            this EcsWorld world, 
            string viewId,
            string layoutType)
        {
            var entity = world.NewEntity();
            
            ref var component = ref world
                .GetOrAddComponent<CreateLayoutViewRequest>(entity);
            
            component.View = viewId;
            component.LayoutType = layoutType;

            return entity;
        }
        
        public static int MakeViewRequest(
            this EcsWorld world, 
            Type viewType,
            ViewType layoutType)
        {
            return MakeViewRequest(world,viewType.Name, layoutType);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MakeViewRequest(
            this EcsWorld world, 
            string view,
            ref EcsPackedEntity target,
            ref EcsPackedEntity owner,
            ViewType layoutType = ViewType.None,
            Transform parent = null,
            string tag = null,
            string viewName = null,
            bool stayWorld = false)
        {
            var layout = layoutType == ViewType.None ? string.Empty : layoutType.ToStringFromCache();
            return MakeViewRequest(world,view,ref target,ref owner, layout, parent, tag, viewName, stayWorld);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MakeViewRequest(
            this EcsWorld world, 
            string view,
            ref EcsPackedEntity target,
            ref EcsPackedEntity owner,
            string layoutType,
            Transform parent = null,
            string tag = null,
            string viewName = null,
            bool stayWorld = false)
        {
            var entity = world.NewEntity();
            ref var component = ref world
                .GetOrAddComponent<CreateViewRequest>(entity);
            
            component.Parent = parent;
            component.Tag = tag;
            component.ViewId = view;
            component.LayoutType = layoutType;
            component.ViewName = viewName;
            component.StayWorld = stayWorld;
            component.Target = target;
            component.Owner = owner;

            return entity;
        }
        
        public static int MakeViewRequest(this EcsWorld world,ref CreateViewRequest request)
        {
            var entity = world.NewEntity();
            ref var component = ref world.GetOrAddComponent<CreateViewRequest>(entity);
            component.Apply(ref request);

            return entity;
        }
        
        public static CreateViewRequest CreateViewRequest(
            string view,
            ViewType layoutType = ViewType.None,
            Transform parent = null,
            string tag = null,
            string viewName = null,
            bool stayWorld = false)
        {
            var layout = layoutType == ViewType.None ? string.Empty : layoutType.ToStringFromCache();
            return CreateViewRequest(view, layout, parent, tag, viewName, stayWorld);
        }
        
        public static CreateViewRequest CreateViewRequest(
            string view,
            string layoutType,
            Transform parent = null,
            string tag = null,
            string viewName = null,
            bool stayWorld = false)
        {
            var component = new CreateViewRequest
            {
                Parent = parent,
                Tag = tag,
                ViewId = view,
                LayoutType = layoutType,
                ViewName = viewName,
                StayWorld = stayWorld
            };

            return component;
        }
        
    }
}
