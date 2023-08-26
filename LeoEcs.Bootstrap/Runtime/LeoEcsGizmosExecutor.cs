namespace UniGame.LeoEcs.Bootstrap.Runtime
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Leopotam.EcsLite;
    using UnityEngine;

    public class LeoEcsGizmosExecutor : MonoBehaviour,ISystemsPlugin
    {
        private bool _isActive;
        private EcsWorld _world;
        private GameObject _executor;
        
        private List<IEcsSystems> _systems = new List<IEcsSystems>();
        private List<IEcsSystem> _allSystems = new List<IEcsSystem>();
        private Dictionary<ILeoEcsGizmosSystem,IEcsSystems> _gizmosSystems = new Dictionary<ILeoEcsGizmosSystem,IEcsSystems>();
        
        public void Dispose()
        {
            _gizmosSystems?.Clear();
            Stop();
#if UNITY_EDITOR
            if (gameObject == null || Application.isPlaying == false)
                return;
#endif
            Destroy(gameObject);
        }

        public bool IsActive => _isActive;

        public void Execute(EcsWorld world)
        {
            _isActive = true;
            _world = world;
        }

        public void Add(IEcsSystems ecsSystems)
        {
#if !UNITY_EDITOR
            return; 
#endif
            if (_systems.Contains(ecsSystems))
                return;

            _systems.Add(ecsSystems);
            
            _allSystems = ecsSystems.GetAllSystems();
            
            foreach (var system in _allSystems)
            {
                if (system is ILeoEcsGizmosSystem gizmosSystem)
                    _gizmosSystems[gizmosSystem] = ecsSystems;
            }
        }

        public void Stop()
        {
            _isActive = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!this) return;
            
            var isActive = _world!=null && 
                           _world.IsAlive() && 
                           Application.isPlaying && 
                           _isActive;
            
            if (!isActive)
                return;
            
            foreach (var systemValue in _gizmosSystems)
            {
                systemValue.Key.RunGizmosSystem(systemValue.Value);
            }
        }
#endif

        private void Awake()
        {
            _systems ??= new List<IEcsSystems>();
            _allSystems ??= new List<IEcsSystem>();
            _gizmosSystems ??= new Dictionary<ILeoEcsGizmosSystem, IEcsSystems>();
        }
    }
}