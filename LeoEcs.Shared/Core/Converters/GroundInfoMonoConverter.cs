namespace Game.Ecs.Core.Converters
{
    using System.Threading;
    using Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Converter.Runtime;
    using UnityEngine;

    public sealed class GroundInfoMonoConverter : MonoLeoEcsConverter
    {
        [SerializeField]
        public float _checkDistance = 0.3f;
        
        public override void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            var groundInfoPool = world.GetPool<GroundInfoComponent>();
            
            ref var groundInfo = ref groundInfoPool.Add(entity);
            groundInfo.CheckDistance = _checkDistance;
        }
    }
}