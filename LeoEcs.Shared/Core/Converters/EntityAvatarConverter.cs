namespace Game.Ecs.Core.Converters
{
    using System.Threading;
    using Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;

    public sealed class EntityAvatarConverter : MonoLeoEcsConverter
    {
        [SerializeField] 
        public EntityBounds _entityBounds;

        [Space]
        [SerializeField]
        public Transform _headRoot;
        [SerializeField] 
        public Transform _bodyRoot;
        [SerializeField]
        public Transform _feetRoot;
        [SerializeField]
        public Transform _handRoot;

        [Space]
        [SerializeField]
        public Transform _weaponRoot;

        public override void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            var avatarPool = world.GetPool<EntityAvatarComponent>();
            ref var avatar = ref avatarPool.GetOrAddComponent(entity);

            avatar.Bounds = _entityBounds;
            
            avatar.Head = _headRoot;
            avatar.Body = _bodyRoot;
            avatar.Feet = _feetRoot;
            avatar.Hand = _handRoot;

            avatar.Weapon = _weaponRoot;
            
            avatar.All = new[]
            {
                _headRoot,
                _bodyRoot,
                _feetRoot,
                _handRoot,
                _weaponRoot
            };
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var transformMatrix = Matrix4x4.TRS(transform.position + _entityBounds.Center, transform.rotation, UnityEditor.Handles.matrix.lossyScale);
            using (new UnityEditor.Handles.DrawingScope(transformMatrix))
            {
                UnityEditor.Handles.color = Color.magenta;

                var pointOffset = (_entityBounds.Height - _entityBounds.Radius * 2) / 2;

                UnityEditor.Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _entityBounds.Radius);
                UnityEditor.Handles.DrawLine(new Vector3(0, pointOffset, -_entityBounds.Radius), new Vector3(0, -pointOffset, -_entityBounds.Radius));
                UnityEditor.Handles.DrawLine(new Vector3(0, pointOffset, _entityBounds.Radius), new Vector3(0, -pointOffset, _entityBounds.Radius));
                UnityEditor.Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _entityBounds.Radius);

                UnityEditor.Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _entityBounds.Radius);
                UnityEditor.Handles.DrawLine(new Vector3(-_entityBounds.Radius, pointOffset, 0), new Vector3(-_entityBounds.Radius, -pointOffset, 0));
                UnityEditor.Handles.DrawLine(new Vector3(_entityBounds.Radius, pointOffset, 0), new Vector3(_entityBounds.Radius, -pointOffset, 0));
                UnityEditor.Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _entityBounds.Radius);

                UnityEditor.Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _entityBounds.Radius);
                UnityEditor.Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _entityBounds.Radius);
            }
#endif
        }
    }
}