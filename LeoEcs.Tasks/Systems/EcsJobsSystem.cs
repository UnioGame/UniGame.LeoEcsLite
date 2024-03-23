namespace Game.Ecs.EcsThreads.Systems
{
    using System;
    using System.Linq;
    using Leopotam.EcsLite;
    using UniGame.Core.Runtime.Extension;
    using UniGame.LeoEcs.Shared.Extensions;
    using UniGame.Runtime.ObjectPool.Extensions;
    using UnityEngine;
    using UnityEngine.Pool;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using Unity.Jobs;

    /// <summary>
    /// unity jobs system support
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class EcsJobsSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        
        

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }
        
        public void Run(IEcsSystems systems)
        {

        }
    }
    
    public struct EcsJob
    {
        
    }
}