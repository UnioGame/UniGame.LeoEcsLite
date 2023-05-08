namespace Game.Ecs.Core.Systems
{
    using System;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Shared.Components;
    
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif
    
    /// <summary>
    /// Add an empty target to an ability
    /// </summary>
    [Serializable]
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public class DestroyNullTransformSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _transformFilter;
        private EcsPool<TransformComponent> _transformPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _transformFilter = _world
                .Filter<TransformComponent>()
                //.Inc<ObjectConverterComponent>()
                .End();

            _transformPool = _world.GetPool<TransformComponent>();

        }

        public void Run(IEcsSystems systems)
        {
            foreach (var transformEntity in _transformFilter)
            {
                ref var transformComponent = ref _transformPool.Get(transformEntity);
                var transform = transformComponent.Value;

                if (transform != null) continue;

                _world.DelEntity(transformEntity);
            }
        }
    }
}