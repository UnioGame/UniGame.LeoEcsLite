namespace Game.Ecs.Core.Timer
{
    using System;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.ExtendedSystems;
    using UniGame.LeoEcs.Timer.Components;
    using UniGame.LeoEcs.Timer.Components.Events;
    using UniGame.LeoEcs.Timer.Components.Requests;
    using Systems;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;

    /// <summary>
    /// base timer feature
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class TimerFeature : ILeoEcsFeature
    {
        public bool enabled = true;

        public bool IsFeatureEnabled => enabled;
        
        public string FeatureName => nameof(TimerFeature);
        
        public UniTask InitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            ecsSystems.DelHere<CooldownFinishedSelfEvent>();
            
            ecsSystems.Add(new RestartTimerSystem());
            ecsSystems.Add(new UpdateActiveTimerStateSystem());
            ecsSystems.Add(new UpdateTimerSystem());

            ecsSystems.DelHere<RestartCooldownSelfRequest>();
            
            return UniTask.CompletedTask;
        }

        public bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return true;
            
            if (ContainsSearchString(nameof(CooldownComponent),searchString)) 
                return true;
            
            return ContainsSearchString(FeatureName, searchString);
        }
        
        protected bool ContainsSearchString(string source, string filter)
        {
            return !string.IsNullOrEmpty(source) && 
                   source.Contains(filter, StringComparison.OrdinalIgnoreCase);
        }
    }

}