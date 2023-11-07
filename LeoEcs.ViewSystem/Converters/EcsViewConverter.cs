namespace UniGame.LeoEcs.ViewSystem.Converters
{
    using System.Threading;
    using Components;
    using Converter.Runtime;
    using Converter.Runtime.Abstract;
    using Converter.Runtime.Components;
    using Leopotam.EcsLite;
    using Shared.Extensions;
    using Sirenix.OdinInspector;
    using UniGame.ViewSystem.Runtime;
    using UniModules.UniGame.UISystem.Runtime;
    using UnityEngine;

    [RequireComponent(typeof(LeoEcsMonoConverter))]
    public class EcsViewConverter : MonoLeoEcsConverter, 
        IEcsViewConverter,
        ILeoEcsComponentConverter
    {
        #region inspector

        [ReadOnly]
        public int entity;

        public bool followEntityLifeTime = false;
        public bool addChildOrderComponent = false;
        
        #endregion
        
        #region private fields

        private EcsWorld _ecsWorld;
        private EcsPackedEntity _viewPackedEntity;
        private IView _view;

        #endregion
        
        #region public properties
        
        public EcsWorld World => _ecsWorld;
        public EcsPackedEntity PackedEntity => _viewPackedEntity;
        public int Entity => entity;
        
        #endregion
        
        #region public methods

        /// <summary>
        /// entity destroyed
        /// </summary>
        public void OnEntityDestroy(EcsWorld world, int targetEntity)
        {
            _ecsWorld = null;
            
            entity = -1;
        }
        
        public sealed override void Apply(GameObject target, EcsWorld world, int targetEntity, CancellationToken cancellationToken = default)
        {
            entity = targetEntity;
            
            _view = GetComponent<IView>();
            
            if (!isActiveAndEnabled && _view == null) return;

            _ecsWorld = world;
            _viewPackedEntity = world.PackEntity(entity);
            
            ref var viewComponent = ref world.GetOrAddComponent<ViewComponent>(entity);
            ref var viewStatusComponent = ref world.GetOrAddComponent<ViewStatusComponent>(entity);
            
            viewStatusComponent.Status = ViewStatus.None;
            viewComponent.View = _view;
            viewComponent.Type = _view.GetType();

            if (addChildOrderComponent)
            {
                ref var childOrderComponent = ref world.GetOrAddComponent<ViewOrderComponent>(entity);
                childOrderComponent.Value = target.transform.GetSiblingIndex();
            }

            //follow entity lifetime  and close view if entity is dead
            if (followEntityLifeTime)
            {
                var lifeTimeEntity = world.NewEntity();
                ref var lifeTimeComponent = ref world.AddComponent<ViewEntityLifeTimeComponent>(lifeTimeEntity);
                lifeTimeComponent.View = _view;
                lifeTimeComponent.Value = _viewPackedEntity;
            }
        }

        #endregion
    }
}