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
            foreach (var entity in entities)
            {
                var idValue = entity.ToStringFromCache();
                if(!idValue.Contains(filterData.filter,StringComparison.OrdinalIgnoreCase))
                    continue;
                filterData.entities.Add(entity);
            }

            return filterData;
        }

    }
}