namespace UniGame.LeoEcs.Debug.Editor
{
    using System;
    using UniModules.UniCore.Runtime.Utils;

    [Serializable]
    public class IdEntitiesFilter : IEcsWorldSearchFilter
    {
        public int[] entities = Array.Empty<int>();

        public EcsFilterData Execute(EcsFilterData filterData)
        {
            var world = filterData.world;
            entities = Array.Empty<int>();
            
            world.GetAllEntities(ref entities);
            var isEmptyFilter = string.IsNullOrEmpty(filterData.filter);
            
            foreach (var entity in entities)
            {
                var idValue = entity.ToStringFromCache();
                var isValid = isEmptyFilter || idValue.Contains(filterData.filter, StringComparison.OrdinalIgnoreCase);
                if(!isValid) continue;
                
                filterData.entities.Add(entity);
            }

            return filterData;
        }

    }
}