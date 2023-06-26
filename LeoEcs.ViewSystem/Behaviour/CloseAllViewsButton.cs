namespace UniGame.LeoEcs.ViewSystem.Behavriour
{
    using System.Threading;
    using Components;
    using Converter.Runtime;
    using Converter.Runtime.Abstract;
    using Core.Runtime;
    using Leopotam.EcsLite;
    using Rx.Runtime.Extensions;
    using Shared.Extensions;
    using Sirenix.OdinInspector;
    using UniModules.UniGame.Core.Runtime.DataFlow.Extensions;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(LeoEcsMonoConverter))]
    public class CloseAllViewsButton : MonoBehaviour, ILeoEcsComponentConverter, ILifeTimeContext
    {
        #region inspector

        public Button trigger;

        #endregion

        private ILifeTime _lifeTime;
        private EcsPackedEntity _packedEntity;
        private EcsWorld _world;

        public ILifeTime LifeTime => _lifeTime;

        public void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            _lifeTime = this.GetAssetLifeTime();
            _world = world;
            _packedEntity = world.PackEntity(entity);
            
            this.Bind(trigger, CloseAll);
        }

        [Button]
        private void CloseAll()
        {
            if (!_packedEntity.Unpack(_world, out var entity)) return;
            _world.GetOrAddComponent<CloseAllViewsRequest>(entity);
        }
        
        [OnInspectorInit]
        private void OnInspectorInitialize()
        {
            if (trigger == null)
                trigger = GetComponent<Button>();
        }

    }
}