namespace Game.Ecs.EcsThreads.Systems
{
    using System;
    using System.Runtime.CompilerServices;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using Unity.Collections;
    using Unity.Mathematics;
    using UnityEngine.Profiling;
    /// <summary>
    /// thread support for ecs system
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class EcsDataTaskSystem<TTask> : IEcsThreadSystem<TTask>
        where TTask : IEcsDataTask<TTask> , new()
    {
        private int _minChunkSize = 32;
        private float _chunkCoeff = 1f;
        
        private bool _isMultithreaded = true;
        private EcsWorld _world;
        private IEcsSystems _ecsSystems;

        TTask _task;
        ThreadWorkerHandler _worker;

        public virtual bool IsMultithreaded => _isMultithreaded;
        
        public void Init(IEcsSystems systems)
        {
            _ecsSystems = systems;
            _world = systems.GetWorld();
            _worker = Execute;
            _task = new TTask();
            
            OnInit(systems);
        }

#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute(int fromIndex, int beforeIndex)
        {
            _task.Execute(fromIndex, beforeIndex);
        }
        
        public void Run(IEcsSystems systems)
        {
            //_task = new TTask();
            var taskCount = SetupTask(ref _task);
            
            if (taskCount<= 0) return;
            
            if(IsMultithreaded)
                TaskThreadService.Run(_worker, taskCount, GetChunkSize(taskCount));
            else
                _task.Execute(0, taskCount);
            
            OnTaskComplete(ref _task);
        }

        public virtual int GetChunkSize(int dataCount) => -1;

        public virtual int SetupTask(ref TTask task) => 0;

        public virtual void OnTaskComplete(ref TTask task) { }

        protected virtual void OnInit(IEcsSystems ecsSystems) {}

    }
    
    public interface IEcsDataTask<TTask> where TTask : IEcsDataTask<TTask>
    {
        void Execute(int fromIndex, int beforeIndex);
    }
    
    public interface IEcsThreadSystem<TTask> : IEcsInitSystem, IEcsRunSystem 
        where TTask : IEcsDataTask<TTask> , new()
    {
        bool IsMultithreaded { get; }
        
        int GetChunkSize(int dataCount);
        
        int SetupTask(ref TTask task);
    }
}