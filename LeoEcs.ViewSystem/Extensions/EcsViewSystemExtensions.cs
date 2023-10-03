namespace UniGame.LeoEcs.ViewSystem.Extensions
{
    using Components;
    using Game.Ecs.UI.EndGameScreens.Systems;
    using LeoEcsLite.LeoEcs.ViewSystem.Systems;
    using Leopotam.EcsLite;
    using UniGame.ViewSystem.Runtime;
    using UniModules.UniCore.Runtime.Utils;
    using UniModules.UniGame.UiSystem.Runtime;

    public static class EcsViewSystemExtensions
    {
        
        public static IEcsSystems ShowQueuedOn<TEvent, TView1, TView2>(
            this IEcsSystems systems,
            ViewType layoutType = ViewType.Window)
            where TEvent : struct
            where TView1 : IView
            where TView2 : IView
        {
            var viewData = new EcsViewData()
            {
                LayoutType = layoutType,
                StayWorld = false,
            };
            
            systems.Add(new ShowQueuedViewOnSystem<TEvent,TView1,TView2>(viewData));
            return systems;
        }
        
        /// <summary>
        /// Show view and mark entity forbidden for same view
        /// </summary>
        public static IEcsSystems ShowSingleOn<TEvent, TView>(
            this IEcsSystems systems,
            ViewType layoutType = ViewType.Window)
            where TEvent : struct
            where TView : IView
        {
            systems.Add(new ShowSingleLayoutViewWhen<TEvent, TView>(layoutType.ToStringFromCache()));
            return systems;
        } 
            
        public static IEcsSystems ShowOn<TView>(
            this IEcsSystems systems,
            EcsFilter filter,
            ViewType layoutType = ViewType.Window)
            where TView : IView
        {
            systems.Add(new ShowLayoutViewWhenSystem<TView>(filter,layoutType.ToStringFromCache()));
            return systems;
        }
        
        public static IEcsSystems CloseOn<TEvent,TView>(this IEcsSystems systems)
            where TEvent : struct
            where TView : IViewModel
        {
            var system = new CloseOnSystem<TEvent, TView>();
            systems.Add(system);
            return systems;
        }
        
        public static IEcsSystems ShowInContainerOn<TEvent,TView>(this IEcsSystems systems,bool useBusy = false,bool ownView = false)
            where TEvent : struct
            where TView : IView
        {
            var data = new ViewContainerSystemData()
            {
                View = typeof(TView).Name,
                UseBusyContainer = useBusy,
                OwnViewBySource = ownView,
            };

            var system = new ShowViewInContainerWhenSystem<TEvent, TView>(data);
            
            systems.Add(system);
            
            return systems;
        }
        
        public static IEcsSystems ShowOn<TEvent, TView>(
            this IEcsSystems systems,
            ViewType layoutType = ViewType.Window)
            where TEvent : struct
            where TView : IView
        {
            systems.Add(new ShowLayoutViewWhenSystem<TEvent, TView>(layoutType.ToStringFromCache()));
            return systems;
        }

        
        public static IEcsSystems ShowOn<TComponent1,TComponent2, TView>(
            this IEcsSystems systems,
            ViewType layoutType = ViewType.Window)
            where TComponent1 : struct
            where TComponent2 : struct
            where TView : IView
        {
            systems.Add(new ShowLayoutViewWhenSystem<TComponent1,TComponent2, TView>(layoutType));
            return systems;
        }

        public static IEcsSystems ShowOn<TView>(
            this IEcsSystems systems,
            EcsFilter filter,
            ViewRequestData viewData)
            where TView : IView
        {
            systems.Add(new ShowViewWhenSystem<TView>(filter,viewData));
            return systems;
        }
        
        public static IEcsSystems ShowOn<TEvent, TView>(
            this IEcsSystems systems,
            ViewRequestData viewData)
            where TEvent : struct
            where TView : IView
        {
            systems.Add(new ShowViewWhenSystem<TEvent, TView>(viewData));
            return systems;
        }
        
        public static IEcsSystems ShowOn<TComponent1,TComponent2, TView>(
            this IEcsSystems systems,
            ViewRequestData viewData)
            where TComponent1 : struct
            where TComponent2 : struct
            where TView : IView
        {
            systems.Add(new ShowViewWhenSystem<TComponent1,TComponent2, TView>(viewData));
            return systems;
        }
    }
}