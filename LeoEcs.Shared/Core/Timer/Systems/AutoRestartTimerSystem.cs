namespace Game.Ecs.Core.Timer.Systems
{
    using System;
    using Leopotam.EcsLite;
    using Time.Service;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using UniGame.LeoEcs.Timer.Components;
    using UniGame.LeoEcs.Timer.Components.Events;

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
    public class AutoRestartTimerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _filter;
        private TimerAspect _timerAspect;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<CooldownActiveComponent>()
                .Inc<CooldownComponent>()
                .Exc<CooldownFinishedSelfEvent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var cooldownComponent = ref _timerAspect.Cooldown.Get(entity);
                ref var stateComponent = ref _timerAspect.State.Get(entity);
                
                var cooldown = cooldownComponent.Value;
                var timePassed = GameTime.Time - stateComponent.LastTime;
                
                if(timePassed < cooldown) continue;
                
                _timerAspect.Active.Del(entity);
                _timerAspect.Completed.Add(entity);
                _timerAspect.Finished.Add(entity);
                
                if(_timerAspect.AutoRestart.Has(entity))
                    _timerAspect.Restart.GetOrAddComponent(entity);
            }
        }
    }
}