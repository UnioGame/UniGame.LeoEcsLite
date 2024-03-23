namespace Game.Ecs.EcsThreads.Systems
{
    using System;
    using Leopotam.EcsLite;
    using UniGame.Core.Runtime;
    using UniModules.UniCore.Runtime.DataFlow;
    using Unity.Collections;
    using Unity.Jobs;

    public abstract class EcsDataJobSystem<TJob,TData,TResult> : IEcsJobDataSystemBase,IEcsInitSystem,IEcsDestroySystem
        where TJob : struct, IEcsDataJob<TData,TResult>
        where TData : unmanaged
        where TResult : unmanaged
    {
        public IEcsSystems ecsSystems;
        public EcsWorld world;
        
        private LifeTimeDefinition _lifeTime;
        
        private int _defaultJobsCount;
        private JobHandle _jobHandle;
        private TJob _job = default;

        public void Init(IEcsSystems systems)
        {
            ecsSystems = systems;
            world = systems.GetWorld();
            
            _lifeTime = new LifeTimeDefinition();
            _defaultJobsCount = 16;
            _jobHandle = default;
            _job = default;
            
            OnInit(systems,_lifeTime);
        }
        
        public void Destroy(IEcsSystems systems)
        {
            _lifeTime.Terminate();
        }
        
        public void Run(IEcsSystems systems)
        {
            ref var jobHandle = ref Schedule();
            
            jobHandle.Complete();
            
            OnJobComplete();
        }

        public ref JobHandle Schedule(JobHandle dependsOn = default(JobHandle))
        {
            var data = GetJobData();
            if (!data.IsCreated || data.Length <= 0) return ref _jobHandle;

            _job = default;
            _job.Init(ref data);
            
            SetJobData(ref _job);
            
            var chunkSize = GetChunkSize();
            _jobHandle = _job.Schedule(data.Length,chunkSize ,dependsOn);
            return ref _jobHandle;
        }
        
        public virtual int GetChunkSize() => _defaultJobsCount;
        
        //custom data setup for job
        public virtual void SetJobData(ref TJob job) { }

        public virtual void OnJobComplete() { }

        public abstract NativeArray<TData> GetJobData();

        protected virtual void OnInit(IEcsSystems systems, ILifeTime lifeTime) { }

    }
    
    
    public interface IEcsJobDataSystemBase : IEcsRunSystem,IEcsInitSystem
    {
        ref JobHandle Schedule(JobHandle dependsOn = default(JobHandle));
        int GetChunkSize();
        void OnJobComplete();
    }
    
    public interface IEcsDataJob<TData,TResult> : IJobParallelFor
        where TData : unmanaged
        where TResult : unmanaged
    {
        void Init(ref NativeArray<TData> data);

        NativeArray<TResult> GetResult();
    }
}