namespace Game.Ecs.EcsThreads.Systems
{
    using System;
    using System.Runtime.CompilerServices;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using Unity.Collections;
    using Unity.Mathematics;

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
    public class EcsDataTaskSystem<TTask,TData,TResult> : IEcsThreadSystem<TTask,TData,TResult>
        where TTask : IEcsDataTask<TData,TResult> , new()
        where TData : struct 
        where TResult : struct
    {
        private int _minChunkSize = 8;
        private int _maxChunkSize = 256;
        private float _chunkCoeff = 1.5f;
        
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
            _task.Initialize(systems);
            
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
            if (!SetupTask(ref _task)) return;
            
            var data = GetTaskData(ref _task);
            if (!data.IsCreated || data.Length <= 0) return;
            
            var dataCount = data.Length;
            var result = new NativeArray<TResult>(dataCount, Allocator.Temp);
            
            _task.SetData(data,result);
            
            if(IsMultithreaded)
                ThreadService.Run(_worker, dataCount, GetChunkSize(dataCount));
            else
                Execute(0, dataCount);
            
            OnTaskComplete(ref _task);
        }

        public virtual int GetChunkSize(int dataCount)
        {
            var procCount = ThreadService.DescsCount;
            var size = dataCount / _chunkCoeff / procCount;
            size = math.max(_minChunkSize, size);
            var chunkSize = (int)math.ceil(size);
            return chunkSize;
        }

        public virtual NativeArray<TData> GetTaskData(ref TTask task) => default;

        public virtual bool SetupTask(ref TTask task) => true;

        public virtual void OnTaskComplete(ref TTask task) { }

        protected virtual void OnInit(IEcsSystems ecsSystems) {}

    }
    
    public interface IEcsDataTask<TData,TResult> 
        where TData : struct
        where TResult : struct
    {
        bool IsComplete { get; }
        
        void Initialize(IEcsSystems systems);
        
        void SetData(NativeArray<TData> data, NativeArray<TResult> result);
        
        void Execute(int fromIndex, int beforeIndex);
    }
    
    public interface IEcsThreadSystem<TTask,TData,TResult> : IEcsInitSystem, IEcsRunSystem 
        where TTask : IEcsDataTask<TData,TResult> , new()
        where TData : struct
        where TResult : struct
    {
        bool IsMultithreaded { get; }
        
        int GetChunkSize(int dataCount);
        
        NativeArray<TData> GetTaskData(ref TTask task);
    }
}