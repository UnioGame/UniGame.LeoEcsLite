namespace UniGame.LeoEcs.Bootstrap.Runtime.PostInitialize
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Core.Runtime;
    using Leopotam.EcsLite;
    using Systems;

    [Serializable]
    public class EcsProfileInitialize : IEcsPostInitializeAction
    {
        public (IEcsSystems value, bool replace) Apply(IEcsSystems ecsSystems,IContext context)
        {
#if ENABLE_ECS_PROFILE
            
            var profileSystems = new List<EcsProfileSystem>();
            var systems = ecsSystems.GetAllSystems();
            
            foreach (var ecsSystem in systems)
            {
                var ecsProfileSystem = new EcsProfileSystem();
                ecsProfileSystem.Initialize(ecsSystem);
                profileSystems.Add(ecsProfileSystem);
            }
            
            systems.Clear();
            systems.AddRange(profileSystems);
            
            return (ecsSystems, true);
#endif
            return (ecsSystems, false);
        }
    }
}