namespace Game.Ecs.Time.Systems
{
    using System;
    using System.Runtime.CompilerServices;
    using Aspects;
    using Components;
    using Leopotam.EcsLite;
    using Service;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UnityEngine;

    /// <summary>
    /// update active timer
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class UpdateEntityTimeSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _filter;
        private TimeAspect _timeAspect;
        private EcsWorld _world;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<EntityGameTimeComponent>()
                .End();
            
            SetGameTime();
        }
        
        public void Run(IEcsSystems systems)
        {
            SetGameTime();
            
            foreach (var entity in _filter)
            {
                ref var timeComponent = ref _timeAspect.GameTime.Get(entity);
                timeComponent.Value += GameTime.DeltaTime;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetGameTime()
        {
            GameTime.Time = Time.time;
            GameTime.DeltaTime = Time.deltaTime;
            GameTime.RealTime = Time.realtimeSinceStartup;
            GameTime.UnscaledTime = Time.unscaledTime;
        }
    }
}
