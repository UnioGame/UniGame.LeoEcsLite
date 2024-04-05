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
    [CreateAssetMenu(menuName = "UniGame/LeoEcs/Systems Update Map",
        fileName = "Systems Update Map")]
    public class LeoEcsUpdateMapAsset : ScriptableObject
    {
        [InlineProperty]
        public List<LeoEcsUpdateQueue> updateQueue = new List<LeoEcsUpdateQueue>();

        [InlineProperty]
        [SerializeReference]
        public List<ILeoEcsSystemsPluginProvider> systemsPlugins = new List<ILeoEcsSystemsPluginProvider>();
        
        [SerializeReference]
        public ILeoEcsUpdateOrderProvider defaultFactory;

    }
}
