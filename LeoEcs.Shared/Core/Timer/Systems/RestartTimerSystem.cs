namespace Game.Ecs.Core.Timer.Systems
{
    using System;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Timer.Components;
    using UniGame.LeoEcs.Timer.Components.Requests;
    using Time.Service;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;

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
    public class RestartTimerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _filter;
        
        private TimerAspect _timerAspect;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<CooldownComponent>()
                .Inc<RestartCooldownSelfRequest>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var cooldownComponent = ref _timerAspect.Cooldown.Get(entity);
                ref var stateComponent = ref _timerAspect.State.GetOrAddComponent(entity);
                
                stateComponent.LastTime = GameTime.Time;
                _timerAspect.Active.GetOrAddComponent(entity);
                _timerAspect.Completed.TryRemove(entity);
                
                ref var remainsTimeComponent = ref _timerAspect.Remains.GetOrAddComponent(entity);
                remainsTimeComponent.Value = cooldownComponent.Value;
            }
        }
    }
}