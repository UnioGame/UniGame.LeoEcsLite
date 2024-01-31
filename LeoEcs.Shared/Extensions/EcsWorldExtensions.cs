namespace UniGame.LeoEcs.Shared.Extensions
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Core.Runtime;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniCore.Runtime.Utils;
    using Unity.IL2CPP.CompilerServices;
    using UnityEngine;

#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public static class EcsWorldExtensions
    {
        private static MemorizeItem<EcsWorld,LifeTimeDefinition> _memorizeItem = MemorizeTool
            .Memorize<EcsWorld, LifeTimeDefinition>(static x =>
            {
                var lifeTime = new LifeTimeDefinition();
                UpdateWorldLifeTime(x,lifeTime).Forget();
                return lifeTime;
                
                static async UniTask UpdateWorldLifeTime(EcsWorld world,LifeTimeDefinition lifeTime)
                {
                    while (world.IsAlive())
                    {
#if UNITY_EDITOR
                        if (!Application.isPlaying)
                        {
                            lifeTime.Terminate();
                            return;
                        }        
#endif
                        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
                    }
      
                    lifeTime.Terminate();
                }
                
            },x => x.Terminate());
        
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnWorldExtensionInit()
        {
            Application.quitting -= Clear;
            Application.quitting += Clear;
            Clear();
        }
        
        public static void Clear()
        {
            _memorizeItem.Dispose();
        }
#endif

#if ENABLE_IL2CPP
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        public static ref TComponent AddComponentToEntity<TComponent>(this EcsWorld world, int entity)
            where TComponent : struct
        {
            var pool = world.GetPool<TComponent>();
            ref var component = ref pool.Add(entity);
            
            return ref component;
        }

#if ENABLE_IL2CPP
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EcsPackedEntity PackedEntity(this int entity, EcsWorld world)
        {
            var packedEntity = world.PackEntity(entity);
            return packedEntity;
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        public static bool AnyHas<TPool>(this EcsWorld world, List<int> entities)
            where TPool : struct
        {
            var pool = world.GetPool<TPool>();
            foreach (var entity in entities)
            {
                if (pool.Has(entity))
                    return true;
            }

            return false;
        }

#if ENABLE_IL2CPP
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TComponent GetComponent<TComponent>(this EcsWorld world, int entity)
            where TComponent : struct
        {
            var pool = world.GetPool<TComponent>();
            return ref pool.Get(entity);
        }

#if ENABLE_IL2CPP
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        public static IEnumerable<EcsPackedEntity> PackAll(this EcsWorld world, IEnumerable<int> entities)
        {
            foreach (var entity in entities)
            {
                yield return world.PackEntity(entity);
            }
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackAll(this EcsWorld world,List<EcsPackedEntity> container, IEnumerable<int> entities)
        {
            foreach (var entity in entities)
            {
                container.Add(world.PackEntity(entity));
            }
        }

#if ENABLE_IL2CPP
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool UnpackAll(this EcsWorld world,List<int> result, List<EcsPackedEntity> packedEntities)
        {
            var unpackResult = true;
            foreach (var ecsPackedEntity in packedEntities)
            {
                if (!ecsPackedEntity.Unpack(world, out var entity))
                {
                    unpackResult = false;
                    continue;
                }
                result.Add(entity);
            }

            return unpackResult;
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UnpackAll(this EcsWorld world,EcsPackedEntity[] from,int[] to)
        {
            var amount = 0;
            foreach (var ecsPackedEntity in from)
            {
                if (!ecsPackedEntity.Unpack(world, out var entity))
                    continue;
                to[amount] = entity;
                amount++;
            }

            return amount;
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PackAll(this EcsWorld world,int[] source,EcsPackedEntity[] result, int count)
        {
            var counter = 0;
            for (var i = 0; i < count; i++)
            {
                var entity = source[i];
                if(entity < 0) continue;
                result[counter] = world.PackEntity(source[i]);
                counter++;
            }
            return counter;
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UnpackAll(this EcsWorld world, EcsPackedEntity[] packedEntities,int[] result, int amount)
        {
            var counter = 0;
            
            for (var i = 0; i < amount; i++)
            {
                ref var packedEntity = ref packedEntities[i];
                if (!packedEntity.Unpack(world, out var entity))
                    continue;
                
                result[counter] = entity;
                counter++;
            }

            return counter;
        }

#if ENABLE_IL2CPP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        public static bool TryRemoveComponent<TComponent>(this EcsWorld world, int entity)
            where TComponent : struct
        {
            var pool = world.GetPool<TComponent>();
            if (!pool.Has(entity))
                return false;

            pool.Del(entity);
            return true;
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
#if ENABLE_IL2CPP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        public static void RemoveComponent<TComponent>(this EcsWorld world, int entity)
            where TComponent : struct
        {
            var pool = world.GetPool<TComponent>();
            pool.Del(entity);
        }

#if ENABLE_IL2CPP
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
#if ENABLE_IL2CPP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        public static bool HasComponent<TComponent>(this EcsWorld world, int entity)
            where TComponent : struct
        {
            var pool = world.GetPool<TComponent>();
            return pool.Has(entity);
        }
        
        
#if ENABLE_IL2CPP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        public static ref TComponent AddComponent<TComponent>(this EcsWorld world, int entity)
            where TComponent : struct
        {
            var pool = world.GetPool<TComponent>();
            ref var component = ref pool.Add(entity);
            return ref component;
        }
        
#if ENABLE_IL2CPP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        public static ref TComponent GetOrAddComponent<TComponent>(this EcsWorld world, int entity)
            where TComponent : struct
        {
            var pool = world.GetPool<TComponent>();
            ref var component = ref pool.Has(entity) ? ref pool.Get(entity) : ref pool.Add(entity);
            return ref component;
        }
        
#if ENABLE_IL2CPP
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        public static ref TComponent AddRawComponent<TComponent>(this EcsWorld world, int entity, TComponent component)
            where TComponent : struct
        {
            var pool = world.GetPoolByType(typeof(TComponent));
            if (pool.Has(entity))
                pool.Del(entity);
            
            pool.AddRaw(entity,component);
            var typePool = world.GetPool<TComponent>();
            return ref typePool.Get(entity);
        }

#if ENABLE_IL2CPP
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        public static void FilterByComponent<T>(this EcsWorld world, IEnumerable<int> filter, List<int> result) where T : struct
        {
            var pool = world.GetPool<T>();
            foreach (var entity in filter)
            {
                if (pool.Has(entity))
                    result.Add(entity);
            }
        }

#if ENABLE_IL2CPP
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        public static void FilterByComponent<T, T2>(this EcsWorld world, IEnumerable<int> filter, List<int> result)
            where T : struct
            where T2 : struct
        {
            var pool = world.GetPool<T>();
            var pool2 = world.GetPool<T2>();
            foreach (var entity in filter)
            {
                if (pool.Has(entity) && pool2.Has(entity))
                    result.Add(entity);
            }
        }

#if ENABLE_IL2CPP
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        public static void FilterByComponent<T, T2, T3>(this EcsWorld world, IEnumerable<int> filter, List<int> result)
            where T : struct
            where T2 : struct
            where T3 : struct
        {
            var pool = world.GetPool<T>();
            var pool2 = world.GetPool<T2>();
            var pool3 = world.GetPool<T3>();
            foreach (var entity in filter)
            {
                if (pool.Has(entity) && pool2.Has(entity) && pool3.Has(entity))
                    result.Add(entity);
            }
        }

#if ENABLE_IL2CPP
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        public static void FilterByComponent<T, T2, T3, T4>(this EcsWorld world, IEnumerable<int> filter, List<int> result)
            where T : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
        {
            var pool = world.GetPool<T>();
            var pool2 = world.GetPool<T2>();
            var pool3 = world.GetPool<T3>();
            var pool4 = world.GetPool<T4>();

            foreach (var entity in filter)
            {
                if (pool.Has(entity) && pool2.Has(entity) && pool3.Has(entity) && pool4.Has(entity))
                    result.Add(entity);
            }
        }

#if ENABLE_IL2CPP
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        public static ILifeTime GetWorldLifeTime(this EcsWorld world) => _memorizeItem[world];

#if ENABLE_IL2CPP
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Il2CppSetOption (Option.NullChecks, false)]
        [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
        public static ILifeTime GetLifeTime(this EcsWorld world) => _memorizeItem[world];
        
    }
}
