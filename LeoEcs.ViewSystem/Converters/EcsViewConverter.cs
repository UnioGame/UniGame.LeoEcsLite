namespace UniGame.LeoEcs.ViewSystem.Converters
{
    using System.Threading;
    using Components;
    using Converter.Runtime;
    using Converter.Runtime.Abstract;
    using Converter.Runtime.Components;
    using Leopotam.EcsLite;
    using Shared.Extensions;
    using UniGame.ViewSystem.Runtime;
    using UnityEngine;

    [RequireComponent(typeof(LeoEcsMonoConverter))]
    public class EcsViewConverter : MonoLeoEcsConverter, IEcsViewConverter,ILeoEcsComponentConverter
    {

        #region inspector

        public int entityId;

        public bool addChildOrderComponent = false;
        
        #endregion
        
        #region private fields

        private bool _isEntityAlive;
        private EcsWorld _ecsWorld;
        private EcsPackedEntity _viewPackedEntity;
        private IView _view;

        #endregion
        
        #region public properties
        
        public bool IsEntityAlive => _isEntityAlive;
        public EcsWorld World => _ecsWorld;
        public EcsPackedEntity PackedEntity => _viewPackedEntity;
        public int Entity => entityId;
        
        #endregion
        
        #region public methods

        /// <summary>
        /// entity destroyed
        /// </summary>
        public void OnEntityDestroy(EcsWorld world, int entity)
        {
            _isEntityAlive = false;
            _ecsWorld = null;
            
            entityId = -1;
        }
        
        public sealed override void Apply(GameObject target, EcsWorld world, int entity, 
            CancellationToken cancellationToken = default)
        {
            _view = GetComponent<IView>();
            
            if (!isActiveAndEnabled && _view == null) return;

            _ecsWorld = world;
            _viewPackedEntity = world.PackEntity(entity);
            
            entityId = entity;
            
            ref var viewComponent = ref world.GetOrAddComponent<ViewComponent>(entity);
            ref var viewStatusComponent = ref world.GetOrAddComponent<ViewStatusComponent>(entity);

            viewComponent.View = _view;
            viewComponent.Type = _view.GetType();

            if (addChildOrderComponent)
            {
                ref var childOrderComponent = ref world.GetOrAddComponent<ViewOrderComponent>(entity);
                childOrderComponent.Order = target.transform.GetSiblingIndex();
            }
            
            
            _isEntityAlive = true;
        }

        #endregion
    }
}