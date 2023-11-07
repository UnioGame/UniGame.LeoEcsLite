namespace UniGame.LeoEcs.ViewSystem.Converters
{
    using System;
    using System.Threading;
    using Components;
    using Converter.Runtime;
    using Converter.Runtime.Abstract;
    using Converter.Runtime.Components;
    using Leopotam.EcsLite;
    using Shared.Extensions;
    using Sirenix.OdinInspector;
    using UiSystem.Runtime;
    using UniGame.ViewSystem.Runtime;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class EcsViewDataConverter<TData> :
        ILeoEcsMonoComponentConverter,
        IConverterEntityDestroyHandler
        where TData : class, IViewModel
    {
        [TitleGroup("settings")]
        public bool followEntityLifeTime = false;
        [TitleGroup("settings")]
        public bool addChildOrderComponent = false;
        [TitleGroup("settings")]
        public bool addUpdateRequestOnCreate = true;
        
        [TitleGroup("runtime")]
        public int entity;
        
        #region private fields

        private EcsWorld _world;
        private EcsPackedEntity _viewPackedEntity;
        private IUiView<TData> _view;
        private LifeTimeDefinition _entityLifeTime;

        #endregion
        
        public bool IsEnabled => true;
        
        public string Name => GetType().Name;

        public void Apply(GameObject target, EcsWorld world, int targetEntity, CancellationToken cancellationToken = default)
        {
            //reset lifetime
            _entityLifeTime ??= new LifeTimeDefinition();
            _entityLifeTime.Release();
            
            _view = target.GetComponent<IUiView<TData>>();
            if (_view == null) return;

            _world = world;
            _viewPackedEntity = world.PackEntity(entity);

            entity = targetEntity;
            
            ref var dataComponent = ref world.GetOrAddComponent<ViewDataComponent<TData>>(entity);
            ref var viewComponent = ref world.GetOrAddComponent<ViewComponent>(entity);
            ref var viewStatusComponent = ref world.GetOrAddComponent<ViewStatusComponent>(entity);

            viewStatusComponent.Status = _view.Status.Value;
            viewComponent.View = _view;
            viewComponent.Type = _view.GetType();

            _view.OnViewModelChanged
                .Subscribe(OnViewModelChanged)
                .AddTo(_entityLifeTime);

            if (_view.Model != null)
                OnViewModelChanged(_view.Model);
            
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
            
            if (addUpdateRequestOnCreate) world.AddComponent<UpdateViewRequest<TData>>(entity);
        }
        
        private void OnViewModelChanged(IViewModel model)
        {
            if(!_viewPackedEntity.Unpack(_world,out var viewEntity))
                return;
            
            ref var modelComponent = ref _world
                .GetOrAddComponent<ViewModelComponent>(viewEntity);
            modelComponent.Model = model;
        }

        public void OnEntityDestroy(EcsWorld world, int viewEntity)
        {
            entity = -1;
            
            _entityLifeTime?.Release();
            _world = null;
            _viewPackedEntity = default;
        }
    }
    
    [Serializable]
    public class ViewOrderConverter : GameObjectConverter
    {
        protected override void OnApply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            ref var dataComponent = ref world.GetOrAddComponent<ViewOrderComponent>(entity);
            dataComponent.Value = target.transform.GetSiblingIndex();
        }
    }
}