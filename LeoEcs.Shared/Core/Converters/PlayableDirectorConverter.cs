﻿namespace Game.Ecs.Core.Converters
{
    using System;
    using System.Threading;
    using Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;
    using UnityEngine.Playables;

    [Serializable]
    public sealed class PlayableDirectorConverter : LeoEcsConverter,IConverterEntityDestroyHandler
    {
        [SerializeField]
        public PlayableDirector playableDirector;
        
        public override void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            var playableDirectorPool = world.GetPool<PlayableDirectorComponent>();
            ref var playableDirectorComponent = ref playableDirectorPool.GetOrAddComponent(entity);
            playableDirectorComponent.Value = playableDirector;
        }
        
        public void OnEntityDestroy(EcsWorld world, int entity)
        {
            world.TryRemoveComponent<PlayableDirectorComponent>(entity);
        }
    }
}