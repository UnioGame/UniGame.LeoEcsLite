namespace UniGame.LeoEcs.ViewSystem.Systems
{
    using System;
    using Bootstrap.Runtime.Attributes;
    using Components;
    using Extensions;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.Di;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class CloseViewSystem : IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsPool<ViewComponent> _viewComponent;

        private EcsFilterInject<Inc<CloseViewSelfRequest,ViewComponent>> _closeFilter;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _closeFilter.Value)
            {
                ref var viewComponent = ref _viewComponent.Get(entity);
                viewComponent.View.Close();
                break;
            }
        }
    }
}