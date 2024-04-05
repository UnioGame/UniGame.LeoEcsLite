using System;
using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

#if TRI_INSPECTOR
    using TriInspector;
#endif

namespace UniGame.LeoEcs.Bootstrap.Runtime.Config
{
    [Serializable]
    [InlineProperty]
    public class LeoEcsUniTaskUpdateProvider : ILeoEcsUpdateOrderProvider
    {
        public LeoEcsPlayerUpdateType updateType = LeoEcsPlayerUpdateType.Update;

        public LeoEcsPlayerUpdateType UpdateType => updateType;
        
        public ILeoEcsExecutor Create() => new LeoEcsExecutor(updateType);
    }
}