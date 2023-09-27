namespace Game.Ecs.Core.Systems
{
    using System;
    using System.Collections.Generic;
    using Components;
    using Death.Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public sealed class ForceValidateDeadChildEntitiesSystem : IEcsRunSystem,IEcsInitSystem
    {
        private EcsWorld _world;
        
        private EcsFilter _requestFilter;
        private EcsFilter _filter;

        private EcsPool<ValidateDeadChildEntitiesRequest> _validatePool;
        private EcsPool<DestroySelfRequest> _destroyPool;
        private EcsPool<OwnerComponent> _ownerPool;
        
        private HashSet<int> _destroyedEntities = new HashSet<int>();
        private HashSet<int> _bufferDestroyedEntities = new HashSet<int>();

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _requestFilter = _world
                .Filter<ValidateDeadChildEntitiesRequest>()
                .End();
            
            _filter = _world
                .Filter<OwnerComponent>()
                .End();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var requestEntity in _requestFilter)
            {
                ref var request = ref _validatePool.Get(requestEntity);
                
                _destroyedEntities.Clear();
                var foundDeadChild = false;

                foreach (var entity in _filter)
                {
                    ref var ownerComponent = ref _ownerPool.Get(entity);
                    if (ownerComponent.Value.Unpack(_world, out var ownerEntity) )
                        continue;
                    _bufferDestroyedEntities.Add(entity);
                    foundDeadChild = true;
                }
                
                if(foundDeadChild == false)
                    break;

                do
                {
                    foundDeadChild = false;
                    var buffer = _bufferDestroyedEntities;
                    _bufferDestroyedEntities = _destroyedEntities;
                    _bufferDestroyedEntities.Clear();
                    _destroyedEntities = buffer;
                    
                    foreach (var entity in _filter)
                    {
                        ref var ownerComponent = ref _ownerPool.Get(entity);
                        if(!ownerComponent.Value.Unpack(_world, out var ownerEntity))
                            continue;
                        
                        if(!_destroyedEntities.Contains(ownerEntity)) continue;

                        _bufferDestroyedEntities.Add(entity);
                        
                        ref var destroyRequest = ref _destroyPool.GetOrAddComponent(entity);
                        destroyRequest.ForceDestroy = request.ForceDestroy;
                        foundDeadChild = true;
                    }
                    
                } while (foundDeadChild);
                
                break;
            }
            
            
        }
    }
}