namespace UniGame.LeoEcs.Converter.Runtime.Converters
{
    using System;
    using System.Threading;
    using Leopotam.EcsLite;
    using Shared.Components;
    using Shared.Extensions;
    using UnityEngine;

    [Serializable]
    public sealed class AnimatorConverter : LeoEcsConverter,IConverterEntityDestroyHandler
    {
        [SerializeField]
        public Animator animator;
        
        public override void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            var animatorPool = world.GetPool<AnimatorComponent>();

            ref var animatorComponent = ref animatorPool.GetOrAddComponent(entity);
            animatorComponent.Value = animator;
        }

        public void OnEntityDestroy(EcsWorld world, int entity)
        {
            world.TryRemoveComponent<AnimatorComponent>(entity);
        }
    }
}