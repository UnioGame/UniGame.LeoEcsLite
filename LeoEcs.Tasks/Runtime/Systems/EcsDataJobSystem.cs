namespace Game.Ecs.EcsThreads.Systems
{
    using Leopotam.EcsLite;
    using UniGame.Core.Runtime;
    using UniModules.UniCore.Runtime.DataFlow;
    using Unity.Jobs;

    public abstract class EcsDataJobSystem<TJob> : IEcsJobDataParallelFor<TJob>,IEcsDestroySystem
        where TJob : struct, IEcsDataJobParallelFor
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
            var defaultHandle = default(JobHandle);
            ref var jobHandle = ref Schedule(systems,ref defaultHandle);
            
            jobHandle.Complete();
            
            UpdateJobResults(ref _job);
        }

        public ref JobHandle Schedule(IEcsSystems systems,ref JobHandle dependsOn)
        {
            _job = default;
            
            var count = UpdateJobData(ref _job);
            if (count <= 0) return ref dependsOn;
            
            var chunkSize = GetChunkSize();
            _jobHandle = _job.Schedule(count,chunkSize ,dependsOn);
            return ref _jobHandle;
        }
        
        public virtual int GetChunkSize() => _defaultJobsCount;
        
        //custom data setup for job
        public virtual int UpdateJobData(ref TJob job) => -1;

        public virtual void UpdateJobResults(ref TJob job) { }

        protected virtual void OnInit(IEcsSystems systems, ILifeTime lifeTime) { }

    }
    
    
    public interface IEcsJobDataParallelFor<TJob> : IEcsRunSystem,IEcsInitSystem
        where TJob : struct, IEcsDataJobParallelFor
    {
        ref JobHandle Schedule(IEcsSystems systems,ref JobHandle dependsOn);
        int GetChunkSize();
        void UpdateJobResults(ref TJob job);
    }
    
    public interface IEcsDataJobParallelFor : IJobParallelFor
    {
    }
}