namespace Game.Ecs.Core.Converters
{
    using System;
    using System.Threading;
    using Components;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;

    [Serializable]
    public sealed class AvatarConverter : LeoEcsConverter
    {
        [SerializeField] 
        [InlineProperty]
        [HideLabel]
        [TitleGroup("Avatar Bounds")]
        public EntityBounds entityBounds;

        [Space]
        [SerializeField]
        public Transform headRoot;
        [SerializeField] 
        public Transform bodyRoot;
        [SerializeField]
        public Transform feetRoot;
        [SerializeField]
        public Transform handRoot;

        [Space]
        [SerializeField]
        public Transform weaponRoot;

        public override void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            var avatarPool = world.GetPool<EntityAvatarComponent>();
            ref var avatar = ref avatarPool.GetOrAddComponent(entity);

            avatar.Bounds = entityBounds;
            avatar.Head = headRoot;
            avatar.Body = bodyRoot;
            avatar.Feet = feetRoot;
            avatar.Hand = handRoot;
            avatar.Weapon = weaponRoot;
        }
    }
}