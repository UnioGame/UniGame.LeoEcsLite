namespace Game.Ecs.Core.Converters
{
    using System.Threading;
    using Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;
    using UnityEngine.Playables;
    
    public sealed class PlayableDirectorMonoConverter : MonoLeoEcsConverter
    {
        [SerializeField]
        private PlayableDirector _playableDirector;
        
        public override void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            var playableDirectorPool = world.GetPool<PlayableDirectorComponent>();
            ref var playableDirectorComponent = ref playableDirectorPool.GetOrAddComponent(entity);

            playableDirectorComponent.Value = _playableDirector;
        }
    }
}