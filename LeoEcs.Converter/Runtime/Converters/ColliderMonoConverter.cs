namespace UniGame.LeoEcs.Converter.Runtime.Converters
{
    using System;
    using System.Threading;
    using Leopotam.EcsLite;
    using Shared.Components;
    using Shared.Extensions;
    using UnityEngine;
    
    public sealed class ColliderMonoConverter : MonoLeoEcsConverter
    {
        [SerializeField]    
        public Collider _collider;
        
        public override void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            ref var colliderComponent = ref world.GetOrAddComponent<ColliderComponent>(entity);
            colliderComponent.Value = _collider;
        }
    }
    
    [Serializable]
    public sealed class ColliderConverter : LeoEcsConverter
    {
        [SerializeField]
        public Collider colliderValue;
        
        public override void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            ref var colliderComponent = ref world.GetOrAddComponent<ColliderComponent>(entity);
            colliderComponent.Value = colliderValue;
        }
    }
}