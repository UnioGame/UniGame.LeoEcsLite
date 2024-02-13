namespace Game.Ecs.Time
{
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using Systems;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/Features/Game Time Feature", fileName = "Game Time Feature")]
    public class GameTimeFeature : BaseLeoEcsFeature
    {
        public override UniTask InitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            ecsSystems.Add(new UpdateEntityTimeSystem());
            return UniTask.CompletedTask;
        }
    }
}
