namespace UniGame.LeoEcs.ViewSystem.Systems
{
    using System;
    using System.Runtime.CompilerServices;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using Components;
    using UniGame.ViewSystem.Runtime;
    using UniModules.UniGame.UiSystem.Runtime;
    using Behavriour;
    using Bootstrap.Runtime.Attributes;
    using Converter.Runtime;
    using Core.Runtime;
    using Game.Ecs.Core.Components;
    using Game.Modules.UnioModules.UniGame.LeoEcsLite.LeoEcs.ViewSystem.Components;
    using Shared.Extensions;

#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class CreateViewSystem : IEcsRunSystem,IEcsInitSystem
    {
        private readonly IGameViewSystem _viewSystem;
        private readonly IEcsViewTools _viewTools;
        private readonly IContext _context;
        
        public EcsFilter _createFilter;
        public EcsWorld _world;

        private EcsPool<CreateViewRequest> _createViewPool;
        private EcsPool<OwnerComponent> _ownerPool;
        private EcsPool<ViewParentComponent> _parentPool;

        public CreateViewSystem(IContext context,IGameViewSystem viewSystem,IEcsViewTools viewTools)
        {
            _context = context;
            _viewSystem = viewSystem;
            _viewTools = viewTools;
        }
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _createFilter = _world.Filter<CreateViewRequest>().End();
            
            _createViewPool = _world.GetPool<CreateViewRequest>();
            _ownerPool = _world.GetPool<OwnerComponent>();
            _parentPool = _world.GetPool<ViewParentComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _createFilter)
            {
                ref var request = ref _createViewPool.Get(entity);
                
                CreateViewByRequest(request).Forget();
            }
        }

        public async UniTask CreateViewByRequest(CreateViewRequest request)
        {
            var viewType = request.ViewId;
            var modelType = _viewSystem.ModelTypeMap.GetViewModelType(viewType);
            var model = await _viewSystem.CreateViewModel(_context, modelType);
            
            var view = request.LayoutType switch
            {
                ViewType.None => await _viewSystem
                    .Create(model,request.ViewId,request.Tag,request.Parent,request.ViewName,request.StayWorld),
                ViewType.Screen => await _viewSystem
                    .OpenScreen(model,request.ViewId,request.Tag,request.ViewName),
                ViewType.Window => await _viewSystem
                    .OpenWindow(model,request.ViewId,request.Tag,request.ViewName),
                ViewType.Overlay => await _viewSystem
                    .OpenOverlay(model,request.ViewId,request.Tag,request.ViewName),
            };

            var entity = await UpdateViewEntity(view);
            if(entity < 0) return;
            
            UpdateViewEntityComponent(entity, model, request);
        }

        private async UniTask<int> UpdateViewEntity(IView view)
        {
            var viewEntity = -1;
            var viewObject = view.GameObject;
            if (viewObject == null) return viewEntity;

            var converter = viewObject.GetComponent<ILeoEcsMonoConverter>();
            if (converter != null)
            {
                if (converter.Entity > 0) return converter.Entity;

                if (converter.AutoCreate)
                {   
                    await UniTask.WaitWhile(() => converter.Entity < 0);
                    return converter.Entity;
                }
                return viewEntity;
            }

            viewEntity = _world.NewEntity();
            viewObject.ConvertGameObjectToEntity(_world, viewEntity);
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