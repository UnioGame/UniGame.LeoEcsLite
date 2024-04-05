
namespace UniGame.LeoEcs.Bootstrap.Runtime.Config
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Collections;
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

#if TRI_INSPECTOR
    using TriInspector;
#endif
    
    [Serializable]
    public class LeoEcsConfigGroup
#if ODIN_INSPECTOR
    : ISearchFilterable
#endif
    {
        [GUIColor(0.2f,0.9f,0f)]
#if ODIN_INSPECTOR
        [ValueDropdown(nameof(GetUpdateIds))]
#endif
        public string updateType;

        [Space(8)]
        [SerializeField]
        [InlineProperty]
#if ODIN_INSPECTOR
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
#endif
        public List<LeoEcsFeatureData> features = new List<LeoEcsFeatureData>();

        public override bool Equals(object obj)
        {
            if (obj is LeoEcsConfigGroup configGroup) return configGroup.updateType == updateType;
            return false;
        }

        public override int GetHashCode() => string.IsNullOrEmpty(updateType) 
            ? 0 
            : updateType.GetHashCode();

        public bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return true;
            foreach (var featureData in features)
            {
                if (featureData.IsMatch(searchString))
                    return true;
            }
            return false;
        }

        public IEnumerable GetUpdateIds()
        {
            return LeoEcsUpdateQueueIds.GetUpdateIds();
        }
        
    }
}