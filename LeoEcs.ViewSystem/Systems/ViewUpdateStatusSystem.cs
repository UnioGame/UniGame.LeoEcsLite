using UniGame.LeoEcs.ViewSystem.Components;

namespace UniGame.LeoEcs.ViewSystem.Systems
{
    using System;
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
            var viewPool = _world.GetPool<ViewComponent>();
            var statusPool = _world.GetPool<ViewStatusComponent>();
            
            foreach (var entity in _viewFilter)
            {
                ref var viewComponent = ref viewPool.Get(entity);
                ref var viewStatusComponent = ref statusPool.Get(entity);

                var view = viewComponent.View;
                viewStatusComponent.Status = view.Status.Value;
            }
        }
    }
}
