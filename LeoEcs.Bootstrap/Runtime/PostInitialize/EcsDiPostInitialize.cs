namespace UniGame.LeoEcs.Bootstrap.Runtime.PostInitialize
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Attributes;
    using Leopotam.EcsLite;
    using UniModules.UniCore.Runtime.ReflectionUtils;

    [Serializable]
    public class EcsDiPostInitialize : IEcsPostInitializeAction
    {
        private Type _diAttributeType = typeof(ECSDIAttribute);
        private List<IEcsDiInjection> _injections = null;

        public EcsDiPostInitialize()
        {
            _injections = new List<IEcsDiInjection>()
            {
                new EcsDiWorldInjection(),
                new EcsDiPoolInjection(),
                new EcsDiAspectInjection(),
            };
        }
        
        public void Apply(IEcsSystems ecsSystems,IEcsSystem system)
        {
            var world = ecsSystems.GetWorld();
            if (world == null) return;
            
            var systemType = system.GetType();
            var isDiSystem = systemType.HasAttribute<ECSDIAttribute>();
            var fields = systemType.GetInstanceFields();
            
            foreach (var field in fields)
            {
                var isDiTarget = isDiSystem || Attribute.IsDefined (field, _diAttributeType);
                if(!isDiTarget) continue;

                foreach (var injection in _injections)
                    injection.ApplyInjection(ecsSystems,field,system,_injections);
            }
        }
        
    }
    
    
}