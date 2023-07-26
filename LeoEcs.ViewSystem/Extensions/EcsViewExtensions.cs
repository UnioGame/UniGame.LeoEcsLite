using System;
using Leopotam.EcsLite;
using UniGame.LeoEcs.Shared.Extensions;
using UniGame.LeoEcs.ViewSystem.Components;
using UniGame.LeoEcs.ViewSystem.Converters;
using UniModules.UniGame.UiSystem.Runtime;
using UnityEngine;

namespace UniGame.LeoEcs.ViewSystem.Extensions
{
    using System.Runtime.CompilerServices;
    using UniGame.ViewSystem.Runtime;

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
        public static EcsFilter CreateViewFilter<TModel>(this EcsWorld world)
            where TModel : IViewModel
        {
            return world.ViewFilter<TModel>().End();
        }


        public static void MakeViewRequest(
            this EcsWorld world,
            Type viewType,
            ViewType layoutType = ViewType.None,
            Transform parent = null,
            string tag = null,
            string viewName = null,
            bool stayWorld = false)
        {
            MakeViewRequest(world, viewType.Name, layoutType, parent, tag, viewName, stayWorld);
        }
        
        public static void MakeViewInContainerRequest<TView>(
            this EcsWorld world, 
            bool useBusyContainer = false,
            EcsPackedEntity owner = default,
            string tag = null,
            string viewName = null,
            bool stayWorld = false)
        {
            world.MakeViewInContainerRequest(typeof(TView).Name, useBusyContainer, owner, tag, viewName, stayWorld);
        }
        
        public static void MakeViewInContainerRequest(
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
        }
        
        public static void MakeViewRequest<TView>(
            this EcsWorld world, 
            ViewType layoutType)
        {
            MakeViewRequest(world,typeof(TView),layoutType);
        }
        
        public static void MakeViewRequest(
            this EcsWorld world, 
            string viewId,
            ViewType layoutType)
        {
            var entity = world.NewEntity();
            
            ref var component = ref world
                .GetOrAddComponent<CreateLayoutViewRequest>(entity);
            
            component.View = viewId;
            component.LayoutType = layoutType;
        }
        
        public static void MakeViewRequest(
            this EcsWorld world, 
            Type viewType,
            ViewType layoutType)
        {
            MakeViewRequest(world,viewType.Name, layoutType);
        }
        
        public static void MakeViewRequest(
            this EcsWorld world, 
            string view,
            ViewType layoutType = ViewType.None,
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
        }
        
        public static void MakeViewRequest(
            this EcsWorld world, 
            CreateViewRequest request)
        {
            var entity = world.NewEntity();
            ref var component = ref world
                .GetOrAddComponent<CreateViewRequest>(entity);
            
            component.Parent = request.Parent;
            component.Tag = request.Tag;
            component.ViewId = request.ViewId;
            component.LayoutType = request.LayoutType;
            component.ViewName = request.ViewName;
            component.StayWorld = request.StayWorld;
        }
        
        public static CreateViewRequest CreateViewRequest(
            string view,
            ViewType layoutType = ViewType.None,
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
