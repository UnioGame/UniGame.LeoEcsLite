namespace Game.Ecs.EcsThreads.Systems
{
    using System;

    public static class ThreadService
    {
        public static readonly int DescsCount;
        
        static ThreadDesc[] _descs;
        static ThreadWorkerHandler _task;

        static ThreadService()
        {
            DescsCount = Environment.ProcessorCount;
            _descs = new ThreadDesc[DescsCount];
            for (var i = 0; i < _descs.Length; i++)
            {
                ref var desc = ref _descs[i];
                desc.Thread = new System.Threading.Thread(ThreadProc) { IsBackground = true };
                desc.HasWork = new System.Threading.ManualResetEvent(false);
                desc.WorkDone = new System.Threading.ManualResetEvent(true);
                desc.Thread.Start(i);
            }
        }

        public static void Run(ThreadWorkerHandler worker, int count, int chunkSize)
        {
#if DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (_task != null)
            {
                throw new System.Exception("Calls from multiple threads not supported.");
            }
#endif
            if (count <= 0 || chunkSize <= 0)
            {
                return;
            }

            _task = worker;
            // _task = task.Execute;
            var processed = 0;
            var jobSize = count / DescsCount;
            int workersCount;
            if (jobSize >= chunkSize)
            {
                workersCount = DescsCount;
            }
            else
            {
                workersCount = count / chunkSize;
                jobSize = chunkSize;
            }

            if (workersCount <= 0)
            {
                workersCount = 1;
            }

            for (int i = 0, iMax = workersCount - 1; i < iMax; i++)
            {
                ref var desc = ref _descs[i];
                desc.FromIndex = processed;
                processed += jobSize;
                desc.BeforeIndex = processed;
                desc.WorkDone.Reset();
                desc.HasWork.Set();
            }

            ref var lastDesc = ref _descs[workersCount - 1];
            lastDesc.FromIndex = processed;
            lastDesc.BeforeIndex = count;
            lastDesc.WorkDone.Reset();
            lastDesc.HasWork.Set();

            for (int i = 0, iMax = workersCount; i < iMax; i++)
            {
                _descs[i].WorkDone.WaitOne();
            }

            _task = null;
        }

        static void ThreadProc(object raw)
        {
            ref var desc = ref _descs[(int)raw];
            try
            {
                while (System.Threading.Thread.CurrentThread.IsAlive)
                {
                    desc.HasWork.WaitOne();
                    desc.HasWork.Reset();
                    _task.Invoke(desc.FromIndex, desc.BeforeIndex);
                    desc.WorkDone.Set();
                }
            }
            catch
            {
                // ignored
            }
        }

        struct ThreadDesc
        {
            public System.Threading.Thread Thread;
            public System.Threading.ManualResetEvent HasWork;
            public System.Threading.ManualResetEvent WorkDone;
            public int FromIndex;
            public int BeforeIndex;
        }
    }
}