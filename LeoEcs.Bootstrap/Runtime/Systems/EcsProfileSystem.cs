namespace UniGame.LeoEcs.Bootstrap.Runtime.Systems
{
    using System;
    using System.Runtime.CompilerServices;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using Unity.Profiling;
    using UnityEngine.Profiling;
    /// <summary>
    /// ecs proxy profile system
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class EcsProfileSystem : IEcsInitSystem, 
        IEcsRunSystem,
        IEcsDestroySystem,
        IEcsPreInitSystem,
        IEcsPostDestroySystem
    {
        private EcsWorld _world;
        private IEcsSystem _system;
        private IEcsRunSystem _runSystem;
        private IEcsDestroySystem _destroySystem;
        private IEcsPostDestroySystem _postDestroySystem;
        private IEcsPreInitSystem _preInitSystem;
        private IEcsInitSystem _initSystem;
        private string _systemName;
        private string _profileTag;
        private ProfilerMarker _marker;
        
        public void Initialize(IEcsSystem system)
        {
            _system = system;
            
            _runSystem = system as IEcsRunSystem;
            _destroySystem = system as IEcsDestroySystem;
            _postDestroySystem = system as IEcsPostDestroySystem;
            _preInitSystem = system as IEcsPreInitSystem;
            _initSystem = system as IEcsInitSystem;
            
            _systemName = system.GetType().GetFormattedName();
            _profileTag = $"ECS.RUN.{_systemName}";
            _marker = new ProfilerMarker(_profileTag);
        }
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _initSystem?.Init(systems);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Run(IEcsSystems systems)
        {
            _marker.Begin();
            _runSystem?.Run(systems);
            _marker.End();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy(IEcsSystems systems)
        {
            _destroySystem?.Destroy(systems);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PreInit(IEcsSystems systems)
        {
            _preInitSystem?.PreInit(systems);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PostDestroy(IEcsSystems systems)
        {
            _postDestroySystem?.PostDestroy(systems);
        }
    }
}