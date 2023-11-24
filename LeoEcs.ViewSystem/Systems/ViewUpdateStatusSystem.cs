using UniGame.LeoEcs.ViewSystem.Components;

namespace UniGame.LeoEcs.ViewSystem.Systems
{
    using System;
    using Aspects;
    using Bootstrap.Runtime.Attributes;
    using Leopotam.EcsLite;
    
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class ViewUpdateStatusSystem : IEcsInitSystem,IEcsRunSystem
    {
        private ViewAspect _viewAspect;
        private EcsFilter _viewFilter;
        private EcsWorld _world;
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _viewFilter = _world
                .Filter<ViewComponent>()
                .Inc<ViewStatusComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _viewFilter)
            {
                ref var viewComponent = ref _viewAspect.View.Get(entity);
                ref var viewStatusComponent = ref _viewAspect.Status.Get(entity);

                var activeStatus = viewStatusComponent.Status;
                var view = viewComponent.View;
                viewStatusComponent.Status = view.Status.Value;

                if (activeStatus != viewStatusComponent.Status)
                    _viewAspect.StatusChanged.Add(entity);
            }
        }
    }
    }
