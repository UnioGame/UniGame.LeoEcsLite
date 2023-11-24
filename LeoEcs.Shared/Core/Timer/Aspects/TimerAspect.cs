namespace UniGame.LeoEcs.Bootstrap.Runtime.Abstract
{
    using System;
    using Game.Ecs.Characteristics.Cooldown.Components;
    using Game.Ecs.Characteristics.Cooldown.Components.Events;
    using Game.Ecs.Characteristics.Cooldown.Components.Requests;
    using Leopotam.EcsLite;

    [Serializable]
    public class TimerAspect : EcsAspect
    {
        public EcsPool<CooldownComponent> Cooldown;
        public EcsPool<CooldownStateComponent> State;
        public EcsPool<CooldownActiveComponent> Active;
        public EcsPool<CooldownCompleteComponent> Completed;
        
        //requests
        public EcsPool<RestartCooldownSelfRequest> Restart;
        
        //events
        public EcsPool<CooldownFinishedSelfEvent> Finished;
    }
}