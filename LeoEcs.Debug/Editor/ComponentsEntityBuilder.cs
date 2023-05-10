namespace UniGame.LeoEcs.Debug.Editor
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using Leopotam.EcsLite;
    using Runtime.ObjectPool;
    using Runtime.ObjectPool.Extensions;
    using Shared.Components;

    [Serializable]
    public class ComponentsEntityBuilder : IEntityEditorViewBuilder
    {
        private EcsWorld _world;
        private EcsPool<NameComponent> _namePool;

        public void Initialize(EcsWorld world)
        {
            _world = world;
            _namePool = _world.GetPool<NameComponent>();
        }

        public void Execute(EntityEditorView view)
        {
            ref var packedEntity = ref view.packedEntity;
            if (!packedEntity.Unpack(_world, out var entity))
            {
                view.isDead = true;
                return;
            }

            if (_namePool.Has(entity))
            {
                ref var nameComponent = ref _namePool.Get(entity);
                view.name = nameComponent.Name;
            }
            
            var componentsCount = _world.GetComponentsCount(entity);
            var components = ArrayPool<object>.Shared.Rent(componentsCount);
            
            _world.GetComponents(view.id, ref components);

            foreach (var component in components)
            {
                if(component == null) continue;
                    
                var componentView = ClassPool.Spawn<ComponentEditorView>();
                componentView.entity = view.id;
                componentView.value = component;
                    
                view.components.Add(componentView);
            }
                
            components.Despawn();
        }
        
        public void Execute(List<EntityEditorView> views)
        {
            foreach (var view in views)
                Execute(view);
        }
    }
}