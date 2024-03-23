namespace Game.Ecs.EcsThreads.Systems
{
    using Leopotam.EcsLite;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Jobs;

    
    public abstract class EcsUnityJobSystem<TJob, T1> : EcsUnityJobSystemBase
        where TJob : struct, IEcsUnityJob<T1>
        where T1 : unmanaged
    {
        EcsFilter _filter;
        EcsPool<T1> _pool1;

        public override void Run(IEcsSystems systems)
        {
            if (_filter == null)
            {
                var world = GetWorld(systems);
                _pool1 = world.GetPool<T1>();
                _filter = GetFilter(world);
            }

            var nEntities = NativeHelpers.WrapToNative(_filter.GetRawEntities());
            var nDense1 = NativeHelpers.WrapToNative(_pool1.GetRawDenseItems());
            var nSparse1 = NativeHelpers.WrapToNative(_pool1.GetRawSparseItems());
            
            TJob job = default;
            job.Init(
                nEntities.Array,
                nDense1.Array, nSparse1.Array);
            SetData(systems, ref job);
            job.Schedule(_filter.GetEntitiesCount(), GetChunkSize(systems)).Complete();
#if UNITY_EDITOR
            NativeHelpers.UnwrapFromNative(nEntities);
            NativeHelpers.UnwrapFromNative(nDense1);
            NativeHelpers.UnwrapFromNative(nSparse1);
#endif
        }

        protected virtual void SetData(IEcsSystems systems, ref TJob job)
        {
        }
    }

    public abstract class EcsUnityJobSystem<TJob, T1, T2> : EcsUnityJobSystemBase
        where TJob : struct, IEcsUnityJob<T1, T2>
        where T1 : unmanaged
        where T2 : unmanaged
    {
        EcsFilter _filter;
        EcsPool<T1> _pool1;
        EcsPool<T2> _pool2;

        public override void Run(IEcsSystems systems)
        {
            if (_filter == null)
            {
                var world = GetWorld(systems);
                _pool1 = world.GetPool<T1>();
                _pool2 = world.GetPool<T2>();
                _filter = GetFilter(world);
            }

            var nEntities = NativeHelpers.WrapToNative(_filter.GetRawEntities());
            var nDense1 = NativeHelpers.WrapToNative(_pool1.GetRawDenseItems());
            var nSparse1 = NativeHelpers.WrapToNative(_pool1.GetRawSparseItems());
            var nDense2 = NativeHelpers.WrapToNative(_pool2.GetRawDenseItems());
            var nSparse2 = NativeHelpers.WrapToNative(_pool2.GetRawSparseItems());
            TJob job = default;
            job.Init(
                nEntities.Array,
                nDense1.Array, nSparse1.Array,
                nDense2.Array, nSparse2.Array);
            SetData(systems, ref job);
            job.Schedule(_filter.GetEntitiesCount(), GetChunkSize(systems)).Complete();
#if UNITY_EDITOR
            NativeHelpers.UnwrapFromNative(nEntities);
            NativeHelpers.UnwrapFromNative(nDense1);
            NativeHelpers.UnwrapFromNative(nSparse1);
            NativeHelpers.UnwrapFromNative(nDense2);
            NativeHelpers.UnwrapFromNative(nSparse2);
#endif
        }

        protected virtual void SetData(IEcsSystems systems, ref TJob job)
        {
        }
    }

    public abstract class EcsUnityJobSystem<TJob, T1, T2, T3> : EcsUnityJobSystemBase
        where TJob : struct, IEcsUnityJob<T1, T2, T3>
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        EcsFilter _filter;
        EcsPool<T1> _pool1;
        EcsPool<T2> _pool2;
        EcsPool<T3> _pool3;

        public override void Run(IEcsSystems systems)
        {
            if (_filter == null)
            {
                var world = GetWorld(systems);
                _pool1 = world.GetPool<T1>();
                _pool2 = world.GetPool<T2>();
                _pool3 = world.GetPool<T3>();
                _filter = GetFilter(world);
            }

            var nEntities = NativeHelpers.WrapToNative(_filter.GetRawEntities());
            var nDense1 = NativeHelpers.WrapToNative(_pool1.GetRawDenseItems());
            var nSparse1 = NativeHelpers.WrapToNative(_pool1.GetRawSparseItems());
            var nDense2 = NativeHelpers.WrapToNative(_pool2.GetRawDenseItems());
            var nSparse2 = NativeHelpers.WrapToNative(_pool2.GetRawSparseItems());
            var nDense3 = NativeHelpers.WrapToNative(_pool3.GetRawDenseItems());
            var nSparse3 = NativeHelpers.WrapToNative(_pool3.GetRawSparseItems());
            TJob job = default;
            job.Init(
                nEntities.Array,
                nDense1.Array, nSparse1.Array,
                nDense2.Array, nSparse2.Array,
                nDense3.Array, nSparse3.Array);
            SetData(systems, ref job);
            job.Schedule(_filter.GetEntitiesCount(), GetChunkSize(systems)).Complete();
#if UNITY_EDITOR
            NativeHelpers.UnwrapFromNative(nEntities);
            NativeHelpers.UnwrapFromNative(nDense1);
            NativeHelpers.UnwrapFromNative(nSparse1);
            NativeHelpers.UnwrapFromNative(nDense2);
            NativeHelpers.UnwrapFromNative(nSparse2);
            NativeHelpers.UnwrapFromNative(nDense3);
            NativeHelpers.UnwrapFromNative(nSparse3);
#endif
        }

        protected virtual void SetData(IEcsSystems systems, ref TJob job)
        {
        }
    }

    public abstract class EcsUnityJobSystem<TJob, T1, T2, T3, T4> : EcsUnityJobSystemBase
        where TJob : struct, IEcsUnityJob<T1, T2, T3, T4>
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        EcsFilter _filter;
        EcsPool<T1> _pool1;
        EcsPool<T2> _pool2;
        EcsPool<T3> _pool3;
        EcsPool<T4> _pool4;

        public override void Run(IEcsSystems systems)
        {
            if (_filter == null)
            {
                var world = GetWorld(systems);
                _pool1 = world.GetPool<T1>();
                _pool2 = world.GetPool<T2>();
                _pool3 = world.GetPool<T3>();
                _pool4 = world.GetPool<T4>();
                _filter = GetFilter(world);
            }

            var nEntities = NativeHelpers.WrapToNative(_filter.GetRawEntities());
            var nDense1 = NativeHelpers.WrapToNative(_pool1.GetRawDenseItems());
            var nSparse1 = NativeHelpers.WrapToNative(_pool1.GetRawSparseItems());
            var nDense2 = NativeHelpers.WrapToNative(_pool2.GetRawDenseItems());
            var nSparse2 = NativeHelpers.WrapToNative(_pool2.GetRawSparseItems());
            var nDense3 = NativeHelpers.WrapToNative(_pool3.GetRawDenseItems());
            var nSparse3 = NativeHelpers.WrapToNative(_pool3.GetRawSparseItems());
            var nDense4 = NativeHelpers.WrapToNative(_pool4.GetRawDenseItems());
            var nSparse4 = NativeHelpers.WrapToNative(_pool4.GetRawSparseItems());
            TJob job = default;
            job.Init(
                nEntities.Array,
                nDense1.Array, nSparse1.Array,
                nDense2.Array, nSparse2.Array,
                nDense3.Array, nSparse3.Array,
                nDense4.Array, nSparse4.Array);
            SetData(systems, ref job);
            job.Schedule(_filter.GetEntitiesCount(), GetChunkSize(systems)).Complete();
#if UNITY_EDITOR
            NativeHelpers.UnwrapFromNative(nEntities);
            NativeHelpers.UnwrapFromNative(nDense1);
            NativeHelpers.UnwrapFromNative(nSparse1);
            NativeHelpers.UnwrapFromNative(nDense2);
            NativeHelpers.UnwrapFromNative(nSparse2);
            NativeHelpers.UnwrapFromNative(nDense3);
            NativeHelpers.UnwrapFromNative(nSparse3);
            NativeHelpers.UnwrapFromNative(nDense4);
            NativeHelpers.UnwrapFromNative(nSparse4);
#endif
        }

        protected virtual void SetData(IEcsSystems systems, ref TJob job)
        {
        }
    }

    public abstract class EcsUnityJobSystemBase : IEcsRunSystem
    {
        public abstract void Run(IEcsSystems systems);
        protected abstract int GetChunkSize(IEcsSystems systems);
        protected abstract EcsFilter GetFilter(EcsWorld world);
        protected abstract EcsWorld GetWorld(IEcsSystems systems);
    }

    public interface IEcsUnityJob<T1> : IJobParallelFor
        where T1 : unmanaged
    {
        void Init(NativeArray<int> entities,
            NativeArray<T1> pool1, NativeArray<int> indices1);
    }

    public interface IEcsUnityJob<T1, T2> : IJobParallelFor
        where T1 : unmanaged
        where T2 : unmanaged
    {
        void Init(
            NativeArray<int> entities,
            NativeArray<T1> pool1, NativeArray<int> indices1,
            NativeArray<T2> pool2, NativeArray<int> indices2);
    }

    public interface IEcsUnityJob<T1, T2, T3> : IJobParallelFor
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        void Init(
            NativeArray<int> entities,
            NativeArray<T1> pool1, NativeArray<int> indices1,
            NativeArray<T2> pool2, NativeArray<int> indices2,
            NativeArray<T3> pool3, NativeArray<int> indices3);
    }

    public interface IEcsUnityJob<T1, T2, T3, T4> : IJobParallelFor
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        void Init(
            NativeArray<int> entities,
            NativeArray<T1> pool1, NativeArray<int> indices1,
            NativeArray<T2> pool2, NativeArray<int> indices2,
            NativeArray<T3> pool3, NativeArray<int> indices3,
            NativeArray<T4> pool4, NativeArray<int> indices4);
    }

    public static class NativeHelpers
    {
        public static unsafe NativeWrappedData<T> WrapToNative<T>(T[] managedData) where T : unmanaged
        {
            fixed (void* ptr = managedData)
            {
#if UNITY_EDITOR
                var nativeData =
                    NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(ptr, managedData.Length,
                        Allocator.None);
                var sh = AtomicSafetyHandle.Create();
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref nativeData, sh);
                return new NativeWrappedData<T> { Array = nativeData, SafetyHandle = sh };
#else
                return new NativeWrappedData<T> { Array =
 NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T> (ptr, managedData.Length, Allocator.None) };
#endif
            }
        }

#if UNITY_EDITOR
        public static void UnwrapFromNative<T1>(NativeWrappedData<T1> sh) where T1 : unmanaged
        {
            AtomicSafetyHandle.CheckDeallocateAndThrow(sh.SafetyHandle);
            AtomicSafetyHandle.Release(sh.SafetyHandle);
        }
#endif
        public struct NativeWrappedData<TT> where TT : unmanaged
        {
            public NativeArray<TT> Array;
#if UNITY_EDITOR
            public AtomicSafetyHandle SafetyHandle;
#endif
        }
    }
}