namespace UniGame.LeoEcs.ViewSystem.Systems
{
    using System;
    using System.Runtime.CompilerServices;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using Components;
    using UniGame.ViewSystem.Runtime;
    using Bootstrap.Runtime.Attributes;
    using Converter.Runtime;
    using Core.Runtime;
    using Game.Ecs.Core.Components;
    using Game.Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.ViewSystem.Components;
    using Leopotam.EcsLite.Di;
    using Shared.Extensions;
    using UiSystem.Runtime;
    using Debug = UnityEngine.Debug;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class CreateViewSystem : IEcsRunSystem
    {
        private readonly IGameViewSystem _viewSystem;
        private readonly IContext _context;
        
        public EcsWorld _world;

        private EcsPool<CreateViewRequest> _createViewPool;
        private EcsPool<OwnerComponent> _ownerPool;
        private EcsPool<ViewParentComponent> _parentPool;

        public EcsFilterInject<Inc<CreateViewRequest>> _createFilter;
        
        public CreateViewSystem(IContext context,IGameViewSystem viewSystem)
        {
            _context = context;
            _viewSystem = viewSystem;
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _createFilter.Value)
            {
                ref var request = ref _createViewPool.Get(entity);
                
                CreateViewByRequest(request).Forget();
            }
        }

        public async UniTask CreateViewByRequest(CreateViewRequest request)
        {
            var viewType = request.ViewId;
            var layoutId = request.LayoutType;
            var modelType = _viewSystem.ModelTypeMap.GetViewModelType(viewType);
            var model = await _viewSystem.CreateViewModel(_context, modelType);
            
            var requestLayout = layoutId;

            if (string.IsNullOrEmpty(layoutId) || 
                GameViewSystem.NoneType.Equals(layoutId, StringComparison.OrdinalIgnoreCase))
                requestLayout = string.Empty;

            if (!string.IsNullOrEmpty(requestLayout) && 
                !_viewSystem.HasLayout(requestLayout))
            {
#if UNITY_EDITOR
                Debug.LogError($"Try to create view {viewType} with layout {layoutId} but layout not found");
#endif
                return;
            }
            
            var view = requestLayout switch
            {
                "" => await _viewSystem
                    .Create(model,request.ViewId,request.Tag,request.Parent,request.ViewName,request.StayWorld),
                _ => await _viewSystem
                    .OpenView(model,request.ViewId,requestLayout,request.Tag,request.ViewName),
            };

            var entity = await UpdateViewEntity(view,request);  
            if(entity<0) return;
            
            var packedEntity = _world.PackEntity(entity);
            if(!packedEntity.Unpack(_world,out var viewEntity)) return;
            
            UpdateViewEntityComponent(entity, model, request);
        }

        private async UniTask<int> UpdateViewEntity(IView view,CreateViewRequest request)
        {
            var viewEntity = -1; 
            var viewObject = view.GameObject;
            if (viewObject == null) return viewEntity;

            if(request.Target.Unpack(_world,out var targetEntity))
                viewEntity = targetEntity;
            
            var converter = viewObject.GetComponent<ILeoEcsMonoConverter>();
            if (converter != null && viewEntity < 0)
            {
                if (converter.Entity > 0) return converter.Entity;
                if (!converter.AutoCreate) return viewEntity;
                
                await UniTask.WaitWhile(() => converter.Entity < 0);
                viewEntity = converter.Entity;
            }
            else
            {
                viewEntity = viewEntity < 0 ? _world.NewEntity() : viewEntity;
                viewObject.ConvertGameObjectToEntity(_world,viewEntity);
            }
            
            if(request.Owner.Unpack(_world,out var ownerEntity))
                _ownerPool.GetOrAddComponent(viewEntity).Value = request.Owner;
            
            return viewEntity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateViewEntityComponent(int viewEntity,IViewModel model,CreateViewRequest request)
        {
            ref var modelComponent = ref _world.GetOrAddComponent<ViewModelComponent>(viewEntity);
            modelComponent.Model = model;

            ref var owner = ref request.Owner;
            ref var parent = ref request.Parent;
            
            if (owner.Unpack(_world, out var ownerEntity)) 
            {
                ref var ownerComponent = ref _ownerPool.GetOrAddComponent(viewEntity);
                ownerComponent.Value = owner;
            }
            
            if (parent != null)
            {
                ref var parentComponent = ref _parentPool.GetOrAddComponent(viewEntity);
                parentComponent.Value = parent;
            }
        }

    }
}