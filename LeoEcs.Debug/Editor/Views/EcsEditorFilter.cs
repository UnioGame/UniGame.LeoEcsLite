namespace UniGame.LeoEcs.Debug.Editor
{
    using System;
    using System.Collections.Generic;
    using Leopotam.EcsLite;

    [Serializable]
    public class EcsEditorFilter
    {
        private HashSet<int> _uniqueEntities = new HashSet<int>();

        private List<IEcsWorldSearchFilter> _ecsResultFilter = new List<IEcsWorldSearchFilter>()
        {
            new CheckEditorStatusFilter(),
            new CheckEcsWorldStatusFilter(),
            new IdEntitiesFilter(),
            new FilterEntitiesComponents(),
        };

        public EcsFilterData Filter(string filterValue,EcsWorld world)
        {
            var filterData = new EcsFilterData
            {
                world = world,
                filter = filterValue,
                message = string.Empty,
                type = ResultType.Success
            };

            var filter = filterData;

            foreach (var searchFilter in _ecsResultFilter)
            {
                filter = searchFilter.Execute(filter);

                if (filter.type == ResultType.Error)
                    break;
            }
            
            return filter;
        }

    }
}