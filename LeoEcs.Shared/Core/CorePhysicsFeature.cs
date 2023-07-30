namespace Game.Ecs.Core
{
    using Systems;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime;

    [UsedImplicitly]
    public sealed class CorePhysicsFeature : LeoEcsSystemAsyncFeature
    {
        public override UniTask InitializeFeatureAsync(EcsSystems ecsSystems)
        {
            ecsSystems.Add(new UpdateGroundInfoSystem());
            
            return UniTask.CompletedTask;
        }
    }
}