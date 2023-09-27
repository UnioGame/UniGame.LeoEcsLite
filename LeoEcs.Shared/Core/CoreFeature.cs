namespace Game.Ecs.Core
{
    using Systems;
    using Components;
    using Cysharp.Threading.Tasks;
    using Death.Components;
    using Death.Systems;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.ExtendedSystems;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using UniGame.LeoEcs.Shared.Components;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/Feature/Core Feature", fileName = "Core Feature")]
    public class CoreFeature : BaseLeoEcsFeature
    {
        public override UniTask InitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            ecsSystems.Add(new ProcessDestroySilentSystem());
            
            ecsSystems.Add(new UpdateRenderStatusSystem());
            
            ecsSystems.Add(new DisableColliderSystem());
            ecsSystems.Add(new ProcessDeadSimpleEntitiesSystem());
            ecsSystems.Add(new ProcessDeadTransformEntitiesSystem());
            
            ecsSystems.Add(new DestroyInvalidChildEntitiesSystem());
            ecsSystems.Add(new ForceValidateDeadChildEntitiesSystem());
            ecsSystems.DelHere<OwnerDestroyedEvent>();
            
            ecsSystems.Add(new CheckInvalidChildEntitiesSystem());
            ecsSystems.Add(new ProcessDespawnSystem());
            
            
            ecsSystems.DelHere<DeadEvent>();
            ecsSystems.DelHere<DisabledEvent>();
            ecsSystems.DelHere<PrepareToDeathEvent>();
            ecsSystems.DelHere<KillEvent>();
            
            ecsSystems.Add(new ProcessKillRequestSystem());
            
            ecsSystems.DelHere<KillRequest>();
            ecsSystems.DelHere<ValidateDeadChildEntitiesRequest>();
            
            ecsSystems.Add(new ProcessDestroySilentSystem());
            
            return UniTask.CompletedTask;
        }
    }
}