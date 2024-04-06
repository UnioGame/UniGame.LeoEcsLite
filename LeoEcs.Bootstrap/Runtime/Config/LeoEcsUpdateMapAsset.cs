using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if TRI_INSPECTOR
using TriInspector;
#endif

namespace UniGame.LeoEcs.Bootstrap.Runtime.Config
{
    [CreateAssetMenu(menuName = "UniGame/LeoEcs/Systems Update Map", fileName = "Systems Update Map")]
    public class LeoEcsUpdateMapAsset : ScriptableObject
    {
#if ODIN_INSPECTOR || TRI_INSPECTOR
        [InlineProperty]
#endif
        public List<LeoEcsUpdateQueue> updateQueue = new List<LeoEcsUpdateQueue>();

#if ODIN_INSPECTOR || TRI_INSPECTOR
        [InlineProperty]
#endif
        [SerializeReference]
        public List<ILeoEcsSystemsPluginProvider> systemsPlugins = new List<ILeoEcsSystemsPluginProvider>();
        
        [SerializeReference]
        public ILeoEcsUpdateOrderProvider defaultFactory;

    }
}
