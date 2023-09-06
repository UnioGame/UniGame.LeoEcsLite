namespace UniGame.LeoEcs.Converter.Runtime.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Abstract;
    using LeoEcsLite.LeoEcs.Shared.Components;
    using Leopotam.EcsLite;
    using Shared.Extensions;
    using Sirenix.OdinInspector;
    using UniCore.Runtime.ProfilerTools;
    using UnityEngine;

    public class ParentComponentsMonoConverter : MonoLeoEcsConverter<ParentComponentsConverter>
    {
    }

    [Serializable]
    public class ParentComponentsConverter : 
        LeoEcsConverter,
        IConverterEntityDestroyHandler
    {
        [Space(8)] 
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)] 
        [InlineEditor()]
        [ListDrawerSettings(ListElementLabelName = "@Name",DefaultExpandedState = false)]
        public List<LeoEcsConverterAsset> configurations = new List<LeoEcsConverterAsset>();
        
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [Space(8)]
        [SerializeReference]
        [ListDrawerSettings(ListElementLabelName = "@Name",DefaultExpandedState = false)]
        public List<ILeoEcsMonoComponentConverter> converters = new List<ILeoEcsMonoComponentConverter>();
        
        public override void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            if (!world.HasComponent<ParentEntityComponent>(entity)) return;
            var parentComponent = world.GetComponent<ParentEntityComponent>(entity);

            if (!parentComponent.Value.Unpack(world, out var parentEntity))
            {
#if UNITY_EDITOR
                GameLog.LogWarning($"{nameof(ParentComponentsConverter)} NONE PARENT ENTITY FOR {entity} : {target}",target);
#endif
                return;
            }

            foreach (var converter in converters)
                converter.Apply(target,world, parentEntity, cancellationToken);

            foreach (var converter in configurations)
                converter.Apply(world, parentEntity, cancellationToken);
        }

        public void OnEntityDestroy(EcsWorld world, int entity)
        {
            foreach (var converter in converters)
            {
                if (converter is IConverterEntityDestroyHandler destroyHandler)
                    destroyHandler.OnEntityDestroy(world, entity);
            }

            foreach (var converter in configurations)
                converter.OnEntityDestroy(world,entity);
        }
    }
}