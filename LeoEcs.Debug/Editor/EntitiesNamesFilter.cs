namespace UniGame.LeoEcs.Debug.Editor
{
    using System;
    using System.Buffers;
    using Runtime.ObjectPool.Extensions;
    using Shared.Components;

    [Serializable]
    public class EntitiesNamesFilter : IEcsWorldSearchFilter
    {
        public int[] entities = Array.Empty<int>();
        public object[] componentsTypesOnEntity = Array.Empty<object>();

        public EcsFilterData Execute(EcsFilterData filterData)
        {
            var world = filterData.world;
            entities = Array.Empty<int>();
            
            world.GetAllEntities(ref entities);
            foreach (var entity in entities)
            {
                if(!IsContainFilteredComponent(entity,filterData))
                    continue;
                filterData.entities.Add(entity);
            }

            return filterData;
        }

        public bool IsContainFilteredComponent(
            int entity,
            EcsFilterData filterData)
        {
            var filter = filterData.filter;
            if (string.IsNullOrEmpty(filter)) return true;
            
            var world = filterData.world;
            var count = world.GetComponentsCount(entity);
            var found = false;
            
            componentsTypesOnEntity = ArrayPool<object>.Shared.Rent(count);
            world.GetComponents(entity,ref componentsTypesOnEntity);

            foreach (var component in componentsTypesOnEntity)
            {
                if(component == null)continue;
                
                var name = component.GetType().Name;
                if (name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    break;
                }

                if (component is GameObjectComponent gameObjectComponent)
                {
                    var gameObject = gameObjectComponent.GameObject;
                    if (gameObject != null)
                    {
                        name = gameObject.name;
                        if (name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                        {
                            found = true;
                            break;
                        }
                    }
                }
                
                if (component is NameComponent nameComponent)
                {
                    name = nameComponent.Name;
                    if (name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
            }

            componentsTypesOnEntity.Despawn();
            return found;
        }
    }
}