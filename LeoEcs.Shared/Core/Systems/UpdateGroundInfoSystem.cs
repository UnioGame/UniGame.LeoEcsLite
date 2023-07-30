namespace Game.Ecs.Core.Systems
{
    using System;
    using Cysharp.Threading.Tasks;
    using Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Shared.Components;
    using UnityEngine;

    public sealed class UpdateGroundInfoSystem : IEcsRunSystem,IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsWorld _world;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world.Filter<GroundInfoComponent>()
                .Inc<TransformComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            var groundInfoPool = _world.GetPool<GroundInfoComponent>();
            var transformPool = _world.GetPool<TransformComponent>();
            
            foreach (var entity in _filter)
            {
                ref var transformComponent = ref transformPool.Get(entity);
                ref var groundInfo = ref groundInfoPool.Get(entity);

                if (Physics.Raycast(transformComponent.Value.position + Vector3.up * 0.1f, Vector3.down, out var hitInfo, groundInfo.CheckDistance))
                {
                    groundInfo.Normal = hitInfo.normal;
                    groundInfo.IsGrounded = true;
                }
                else
                {
                    groundInfo.Normal = Vector3.up;
                    groundInfo.IsGrounded = false;
                }
            }
        }
    }
}