namespace UniGame.LeoEcs.Bootstrap.Runtime.Config
{
    using System.Collections.Generic;
    using Abstract;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/LeoEcs/ECS Features Configuration", fileName = nameof(LeoEcsFeaturesConfiguration))]
    public class LeoEcsFeaturesConfiguration : ScriptableObject, ILeoEcsSystemsConfig
    {
        [FoldoutGroup("world config")]
        [InlineProperty]
        [HideLabel]
        public EcsWorldConfiguration worldConfiguration = new EcsWorldConfiguration();
        
        [Space(8)]
        [SerializeField]
        //[Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [ListDrawerSettings(ListElementLabelName = "updateType")]
        public List<LeoEcsConfigGroup> ecsUpdateGroups = new List<LeoEcsConfigGroup>();
        
        public IReadOnlyList<LeoEcsConfigGroup> FeatureGroups => ecsUpdateGroups;
    }
}