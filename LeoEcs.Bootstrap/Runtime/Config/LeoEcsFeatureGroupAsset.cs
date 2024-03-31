namespace UniGame.LeoEcs.Bootstrap.Runtime.Config
{
    using Cysharp.Threading.Tasks;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Abstract;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;
    using UniCore.Runtime.ProfilerTools;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/LeoEcs/Features/ECS Feature", fileName = "ECS Feature")]
    public class LeoEcsFeatureGroupAsset : 
        BaseLeoEcsFeature, 
        ILeoEcsSystemsGroup
    {
        #region inspector
        
        [InlineProperty]
        [HideLabel]
        public LeoEcsSystemsGroupConfiguration groupConfiguration = new LeoEcsSystemsGroupConfiguration();

        #endregion

        #region public properties

        public IReadOnlyList<IEcsSystem> EcsSystems => groupConfiguration.EcsSystems;
        
        public void RegisterSystems(List<IEcsSystem> systems)
        {
            systems.AddRange(EcsSystems);
        }

        public override string FeatureName => string.IsNullOrEmpty(groupConfiguration.FeatureName)
            ? name
            : groupConfiguration.FeatureName;
        
        public override bool IsFeatureEnabled => groupConfiguration.IsFeatureEnabled;

        public override bool ShowFeatureInfo => false;


        #endregion
        
        public sealed override async UniTask InitializeFeatureAsync(IEcsSystems ecsSystems)
        {
#if DEBUG
            var timer = Stopwatch.StartNew();   
            timer.Restart();
#endif
            await OnInitializeFeatureAsync(ecsSystems);
#if DEBUG
            var elapsed = timer.ElapsedMilliseconds;
            timer.Stop();
            GameLog.LogRuntime($"\tECS FEATURE SOURCE: SELF LOAD TIME {FeatureName} | {GetType().Name} = {elapsed} ms");
#endif
            await groupConfiguration.InitializeFeatureAsync(ecsSystems);
            await OnPostInitializeFeatureAsync(ecsSystems);
            
#if DEBUG
            GameLog.LogRuntime($"\n");
#endif
        }

        public override bool IsMatch(string searchString)
        {
            if (base.IsMatch(searchString)) return true;

            return groupConfiguration != null && 
                   groupConfiguration.IsMatch(searchString);
        }
        
        protected virtual UniTask OnInitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            return UniTask.CompletedTask;
        }
        
        protected virtual UniTask OnPostInitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            return UniTask.CompletedTask;
        }
        
        [OnInspectorInit]
        private void OnInspectorInitialize()
        {
            if (groupConfiguration != null && 
                string.IsNullOrEmpty(groupConfiguration.FeatureName))
                groupConfiguration.name = name;
        }
    }
}