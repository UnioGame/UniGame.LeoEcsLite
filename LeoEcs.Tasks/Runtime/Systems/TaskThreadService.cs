namespace Game.Ecs.EcsThreads.Systems
{
    using System;
    using System.Buffers;
    using System.Collections.Concurrent;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.Serialization;

    public static class TaskThreadService
    {
        public static int DescsCount;
        public static int MainThread;
        public static int WorkersCount;
        public static int MinChunkSize = 8;

        static ThreadDesc[] _descs;
        static TaskDesc[] _queuedTasks;
        static int _queuedTasksCount;
        static ConcurrentQueue<int> _freeWorkers = new();
        static ConcurrentQueue<TaskDesc> _tasks = new();
        static ThreadWorkerHandler _task;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void SetupTaskService()
        {
            if (_descs != null)
            {
                for (var i = 0; i < _descs.Length; i++)
                {
                    ref var desc = ref _descs[i];
                    desc.HasWork.Dispose();
                    if(desc.Thread.IsAlive)
                        desc.Thread.Abort();
                }
            }

            MainThread = Thread.CurrentThread.ManagedThreadId;
            DescsCount = Environment.ProcessorCount;
            WorkersCount = DescsCount;

            _descs = new ThreadDesc[DescsCount];
            
            for (var i = 0; i < _descs.Length; i++)
            {
                ref var desc = ref _descs[i];
                desc.Tasks = _tasks;
                desc.HasWork = new ManualResetEventSlim(false, 10);
                desc.Thread = new Thread(ThreadProc) { IsBackground = true };
                desc.Thread.Start(i);
                
                _freeWorkers.Enqueue(i);
            }
        }
        
        public static void Run(ThreadWorkerHandler worker, int count, int chunkSize)
        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (_task != null)
            {
                throw new Exception("Calls from multiple threads not supported.");
            }
#endif
            if (count <= 0) return;

            _task = worker;
            
            WorkersCount = math.max(WorkersCount, 1);

            chunkSize = chunkSize < 0 ? count/WorkersCount : chunkSize;
            chunkSize = math.max(chunkSize, MinChunkSize);

            var jobsCount = count / chunkSize;

            if (jobsCount < WorkersCount)
            {
                chunkSize = count / WorkersCount;
                chunkSize = math.max(1, chunkSize);
            }

            FillJobsQueue(count, chunkSize, WorkersCount);

            foreach (var threadDesc in _descs)
                threadDesc.HasWork.Set();
            
            var spin = new SpinWait();
            var done = false;
            do
            {
                done = true;
                
                for (int i = 0; i < _queuedTasksCount && done; i++)
                {
                    ref var taskDesc = ref _queuedTasks[i];
                    done &= taskDesc.IsCompleted;
                }

                if (!done)
                {
                    //spin.SpinOnce();
                }
                
            } while (!done);

            foreach (var threadDesc in _descs)
                threadDesc.HasWork.Reset();
            
            _task = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FillJobsQueue(int count, int chunkSize, int workersCount)
        {
            if(_queuedTasks!=null)
                ArrayPool<TaskDesc>.Shared.Return(_queuedTasks);
    
            _queuedTasks = ArrayPool<TaskDesc>.Shared.Rent(count);
            _queuedTasksCount = 0;

            var processed = 0;

            while (processed < count)
            {
                var fromIndex = processed;
                var beforeIndex = processed + chunkSize;
                processed = beforeIndex;

                beforeIndex = math.min(count, beforeIndex);
                
                var taskDesk = new TaskDesc()
                {
                    FromIndex = fromIndex,
                    BeforeIndex = beforeIndex,
                    Index = _queuedTasksCount,
                    IsCompleted = false
                };
                
                _queuedTasks[_queuedTasksCount] = taskDesk;
                _queuedTasksCount++;
                
                _tasks.Enqueue(taskDesk);
            }
        }

        static void ThreadProc(object raw)
        {
            var spinMaxValue = 5;
            var spinCounter = 0;
            var spin = new SpinWait();

            ref var desc = ref _descs[(int)raw];
            
            try
            {
                while (Thread.CurrentThread.IsAlive)
                {
                    desc.HasWork.Wait();
                    
                    if (desc.Tasks.TryDequeue(out var task))
                    {
                        desc.IsActive = true;
                        
                        _task.Invoke(task.FromIndex, task.BeforeIndex);
                        
                        ref var taskDesc = ref _queuedTasks[task.Index];
                        taskDesc.IsCompleted = true;
                        
                        continue;
                    }

                    desc.IsActive = false;
                    spin.SpinOnce();
                    // if (spinCounter < spinMaxValue)
                    // {
                    //     spinCounter++;
                    //     spin.SpinOnce();
                    //     continue;
                    // }
                    //
                    // spinCounter = 0;
                    // Thread.Yield();
                }
            }
            catch
            {
                // ignored
            }
        }

        struct ThreadDesc
        {
            public Thread Thread;
            public ManualResetEventSlim HasWork;
            public ConcurrentQueue<TaskDesc> Tasks;
            public int FromIndex;
            public int BeforeIndex;
            public bool IsActive;
        }

        struct TaskDesc
        {
            public int Index;
            public bool IsCompleted;
            public int FromIndex;
            public int BeforeIndex;
        }
    }
}