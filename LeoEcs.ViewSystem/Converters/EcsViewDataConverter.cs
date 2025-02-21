namespace UniGame.LeoEcs.ViewSystem.Converters
{
    using System;
    using Components;
    using Converter.Runtime;
    using Converter.Runtime.Components;
    using Leopotam.EcsLite;
    using Shared.Extensions;
    using Sirenix.OdinInspector;
    using UiSystem.Runtime;
    using UniGame.ViewSystem.Runtime;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniRx;
    using UnityEngine;
    using ViewSkinTagComponent = ViewSkinTagComponent;

    [Serializable]
    public class EcsViewDataConverter<TData> :
        GameObjectConverter,
        IConverterEntityDestroyHandler
        where TData : class, IViewModel
    {
        [TitleGroup("settings")]
        public EcsViewSettings settings = new EcsViewSettings();
        
        [TitleGroup("runtime")]
        public int entity;
        
        #region private fields

        private EcsWorld _world;
        private EcsPackedEntity _viewPackedEntity;
        private IUiView<TData> _view;
        private ViewSkinTagComponent _viewSkinTag;
        private LifeTimeDefinition _entityLifeTime;

        #endregion
        
        public bool IsEnabled => true;
        
        public string Name => GetType().Name;

        public void SetUp(EcsViewSettings overrideSettings)
        {
            settings = overrideSettings;
        }
        
        protected override void OnApply(GameObject target, EcsWorld world, int targetEntity)
        {
            //reset lifetime
            _entityLifeTime ??= new LifeTimeDefinition();
            _entityLifeTime.Release();
            
            _viewSkinTag = target.GetComponent<ViewSkinTagComponent>();
            _view = target.GetComponent<IUiView<TData>>();
            if (_view == null) return;

            entity = targetEntity;
            
            _world = world;
            _viewPackedEntity = world.PackEntity(entity);
            
            ref var dataComponent = ref world.GetOrAddComponent<ViewDataComponent<TData>>(entity);
            ref var viewComponent = ref world.GetOrAddComponent<ViewComponent>(entity);
            ref var viewStatusComponent = ref world.GetOrAddComponent<ViewStatusComponent>(entity);

            viewStatusComponent.Status = _view.Status.Value;
            viewComponent.View = _view;
            viewComponent.Type = _view.GetType();

            if (_viewSkinTag)
            {
                ref var viewSkinTagComponent = ref world.GetOrAddComponent<Components.ViewSkinTagComponent>(entity);
                viewSkinTagComponent.SkinId = _viewSkinTag.skinTag;
            }

            
            _view.OnViewModelChanged
                .Subscribe(OnViewModelChanged)
                .AddTo(_entityLifeTime);

            if (_view.Model != null)
                OnViewModelChanged(_view.Model);
            
            if (settings.addChildOrderComponent)
            {
                ref var childOrderComponent = ref world.GetOrAddComponent<ViewOrderComponent>(entity);
                childOrderComponent.Value = target.transform.GetSiblingIndex();
            }

            //follow entity lifetime  and close view if entity is dead
            if (settings.followEntityLifeTime)
            {
                var lifeTimeEntity = world.NewEntity();
                ref var lifeTimeComponent = ref world.AddComponent<ViewEntityLifeTimeComponent>(lifeTimeEntity);
                lifeTimeComponent.View = _view;
                lifeTimeComponent.Value = _viewPackedEntity;
            }
        }
        
        private void OnViewModelChanged(IViewModel model)
        {
            if(!_viewPackedEntity.Unpack(_world,out var viewEntity))
                return;

            if (settings.addUpdateRequestOnCreate)
            {
                _world.GetOrAddComponent<UpdateViewRequest>(entity);
            }
            
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
}