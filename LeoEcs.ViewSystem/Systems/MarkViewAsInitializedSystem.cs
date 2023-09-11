namespace UniGame.LeoEcs.ViewSystem.Systems
{
    using System;
    using Bootstrap.Runtime.Attributes;
    using Components;
    using Extensions;
    using Leopotam.EcsLite;
    using Shared.Extensions;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class MarkViewAsInitializedSystem : IEcsRunSystem,IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsWorld _world;
        private EcsPool<ViewInitializedComponent> _viewInitialized;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<ViewComponent>()
                .Inc<ViewModelComponent>()
                .Exc<ViewInitializedComponent>()
                .End();
            
            _viewInitialized = _world.GetPool<ViewInitializedComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var viewEntity in _filter)
            {
                _world.GetOrAddComponent<ViewInitializedComponent>(viewEntity);
            }
        }

    }
}