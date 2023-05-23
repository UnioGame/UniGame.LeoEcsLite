using System;
using Cysharp.Threading.Tasks;
using Leopotam.EcsLite;
using UniGame.LeoEcs.ViewSystem.Components;
using UniGame.LeoEcs.ViewSystem.Converters;
using UniGame.ViewSystem.Runtime;
using UniModules.UniGame.UiSystem.Runtime;
using Unity.IL2CPP.CompilerServices;

namespace UniGame.LeoEcs.ViewSystem.Systems
{
    using System.Threading;
    using Behavriour;
    using Converter.Runtime;
    using Converter.Runtime.Abstract;
    using Core.Runtime;
    using Shared.Extensions;

#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class CreateViewSystem : IEcsRunSystem,IEcsInitSystem
    {
        private readonly IGameViewSystem _viewSystem;
        private readonly IEcsViewTools _viewTools;
        private readonly IContext _context;
        
        public EcsFilter _createFilter;
        public EcsWorld _world;

        private EcsPool<CreateViewRequest> _createViewPool;

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

            var viewObject = view.GameObject;
            if (viewObject == null) return;

            var converter = viewObject.GetComponent<ILeoEcsMonoConverter>();
            if (converter == null) return;
            
            converter.RegisterDynamicCallback(x => ConvertView(x,model));
        }

        public void ConvertView(EcsPackedEntity viewPackedEntity,IViewModel model)
        {
            if (!viewPackedEntity.Unpack(_world, out var viewEntity))
                return;
            
            _world.GetOrAddComponent<ViewInitializedComponent>(viewEntity);
            _viewTools.AddViewModelData(_world,viewPackedEntity,model);
        }
    }
}