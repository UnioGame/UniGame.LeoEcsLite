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
    using Game.Ecs.UI.EndGameScreens.Systems;
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
        
        public static void MakeChildViewRequest(this IEcsView view, Type viewType,
            string tag = null,
            string viewName = null,
            Transform parent = null,
            bool stayWorld = false)
        {
            
        }
        
        /// <summary>
        /// Show view and mark entity forbidden for same view
        /// </summary>
        public static EcsSystems ShowSingleOn<TEvent, TView>(
            this EcsSystems systems,
            ViewType layoutType = ViewType.Window)
            where TEvent : struct
            where TView : IView
        {
            systems.Add(new ShowSingleLayoutViewWhen<TEvent, TView>(layoutType));
            return systems;
        } 
            
        public static EcsSystems ShowOn<TView>(
            this EcsSystems systems,
            EcsFilter filter,
            ViewType layoutType = ViewType.Window)
            where TView : IView
        {
            systems.Add(new ShowLayoutViewWhen<TView>(filter,layoutType));
            return systems;
        }
        
        public static EcsSystems ShowOn<TEvent, TView>(
            this EcsSystems systems,
            ViewType layoutType = ViewType.Window)
            where TEvent : struct
            where TView : IView
        {
            systems.Add(new ShowLayoutViewWhen<TEvent, TView>(layoutType));
            return systems;
        }

        
        public static EcsSystems ShowOn<TComponent1,TComponent2, TView>(
            this EcsSystems systems,
            ViewType layoutType = ViewType.Window)
            where TComponent1 : struct
            where TComponent2 : struct
            where TView : IView
        {
            systems.Add(new ShowLayoutViewWhen<TComponent1,TComponent2, TView>(layoutType));
            return systems;
        }

        public static EcsSystems ShowOn<TView>(
            this EcsSystems systems,
            EcsFilter filter,
            ViewRequestData viewData)
            where TView : IView
        {
            systems.Add(new ShowViewWhen<TView>(filter,viewData));
            return systems;
        }
        
        public static EcsSystems ShowOn<TEvent, TView>(
            this EcsSystems systems,
            ViewRequestData viewData)
            where TEvent : struct
            where TView : IView
        {
            systems.Add(new ShowViewWhen<TEvent, TView>(viewData));
            return systems;
        }
        
        public static EcsSystems ShowOn<TComponent1,TComponent2, TView>(
            this EcsSystems systems,
            ViewRequestData viewData)
            where TComponent1 : struct
            where TComponent2 : struct
            where TView : IView
        {
            systems.Add(new ShowViewWhen<TComponent1,TComponent2, TView>(viewData));
            return systems;
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
            var entity = world.NewEntity();
            
            ref var component = ref world
                .GetOrAddComponent<CreateViewRequest>(entity);
            
            component.Parent = parent;
            component.Tag = tag;
            component.Type = viewType;
            component.LayoutType = layoutType;
            component.ViewName = viewName;
            component.StayWorld = stayWorld;
        }
        
    }
}
