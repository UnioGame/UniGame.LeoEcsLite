namespace UniGame.LeoEcs.Converter.Runtime.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Abstract;
    using LeoEcsLite.LeoEcs.Shared.Components;
    using Leopotam.EcsLite;
    using Shared.Components;
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
        public List<IEcsComponentConverter> converters = new List<IEcsComponentConverter>();

        private int _parentEntity = -1;
        
        public override void Apply(GameObject target, EcsWorld world, int entity)
        {
            if (!world.HasComponent<ParentEntityComponent>(entity)) return;
            var parentComponent = world.GetOrAddComponent<ParentEntityComponent>(entity);

            if (!parentComponent.Value.Unpack(world, out var parentEntity))
            {
#if UNITY_EDITOR
                GameLog.LogWarning($"{nameof(ParentComponentsConverter)} NONE PARENT ENTITY FOR {entity} : {target}",target);
#endif
                return;
            }

            ref var gameObjectComponent = ref world
                .GetOrAddComponent<GameObjectComponent>(entity);
            gameObjectComponent.Value = target;
            
            _parentEntity = parentEntity;
            
            foreach (var converter in converters)
                converter.Apply(world, _parentEntity);

            foreach (var converter in configurations)
                converter.Apply(world, _parentEntity);
        }

        public void OnEntityDestroy(EcsWorld world, int entity)
        {
            var packedParent = world.PackEntity(_parentEntity);
            if(!packedParent.Unpack(world,out var parentEntity)) return;
            
            foreach (var converter in converters)
            {
                if (converter is IConverterEntityDestroyHandler destroyHandler)
                    destroyHandler.OnEntityDestroy(world, parentEntity);
            }

            foreach (var converter in configurations)
                converter.OnEntityDestroy(world,parentEntity);
        }
    }
}