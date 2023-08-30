namespace UniGame.LeoEcs.Converter.Runtime.Converters
{
    using System.Threading;
    using Leopotam.EcsLite;
    using Shared.Components;
    using Shared.Extensions;
    using UnityEngine;

    public sealed class AnimatorConverter : MonoLeoEcsConverter
    {
        [SerializeField]
        private Animator _animator;
        
        public override void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            var animatorPool = world.GetPool<AnimatorComponent>();

            ref var animator = ref animatorPool.GetOrAddComponent(entity);
            animator.Value = _animator;
        }
    }
}