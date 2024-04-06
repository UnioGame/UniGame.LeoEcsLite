namespace UniGame.LeoEcs.Bootstrap.Runtime.Config
{
    using System.Collections.Generic;
    using Abstract;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

#if TRI_INSPECTOR
    using TriInspector;
#endif
    
    [CreateAssetMenu(menuName = "UniGame/LeoEcs/ECS Features Configuration", fileName = nameof(LeoEcsFeaturesConfiguration))]
    public class LeoEcsFeaturesConfiguration : ScriptableObject, ILeoEcsSystemsConfig
    {
#if ODIN_INSPECTOR
       [FoldoutGroup("world config")]
#endif
#if ODIN_INSPECTOR || TRI_INSPECTOR
        [InlineProperty]
        [HideLabel]
#endif
        public EcsWorldConfiguration worldConfiguration = new EcsWorldConfiguration();
        
        [Space(8)]
        [SerializeField]
#if ODIN_INSPECTOR
        [ListDrawerSettings(ListElementLabelName = "updateType")]
#endif
        //[Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        public List<LeoEcsConfigGroup> ecsUpdateGroups = new List<LeoEcsConfigGroup>();
        
        public IReadOnlyList<LeoEcsConfigGroup> FeatureGroups => ecsUpdateGroups;
    }
}