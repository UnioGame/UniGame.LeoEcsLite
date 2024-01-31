namespace Game.Ecs.Core
{
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using Systems;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/Features/Core/Core Late Feature", fileName = "Core Late Feature")]
    public sealed class CoreLateUpdateFeature : BaseLeoEcsFeature
    {
        public override UniTask InitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            ecsSystems.Add(new AddTransformComponentsSystem());
            ecsSystems.Add(new UpdateTransformDataSystem());
            
            return UniTask.CompletedTask;
        }
    }
}