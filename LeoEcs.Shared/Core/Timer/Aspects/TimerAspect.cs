namespace UniGame.LeoEcs.Bootstrap.Runtime.Abstract
{
    using System;
    using Timer.Components;
    using Timer.Components.Events;
    using Timer.Components.Requests;
    using Leopotam.EcsLite;

    [Serializable]
    public class TimerAspect : EcsAspect
    {
        public EcsPool<CooldownComponent> Cooldown;
        public EcsPool<CooldownStateComponent> State;
        public EcsPool<CooldownActiveComponent> Active;
        public EcsPool<CooldownCompleteComponent> Completed;
        public EcsPool<CooldownAutoRestartComponent> AutoRestart;
        
        //requests
        public EcsPool<RestartCooldownSelfRequest> Restart;
        
        //events
        public EcsPool<CooldownFinishedSelfEvent> Finished;
    }
}