﻿namespace UniGame.LeoEcs.Shared.Extensions
{
    using System.Runtime.CompilerServices;
    using Leopotam.EcsLite;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public static class EcsPoolExtensions
    {
#if ENABLE_IL2CPP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool TryAdd<T>(this EcsPool<T> pool,ref EcsPackedEntity packedEntity) where T : struct
        {
            var world = pool.GetWorld();
            if (!packedEntity.Unpack(world, out var entity) || pool.Has(entity))
                return false;

            pool.Add(entity);
            return true;
        }
        
#if ENABLE_IL2CPP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool TryAdd<T>(this EcsPool<T> pool, int entity) where T : struct
        {
            if (pool.Has(entity)) 
                return false;
            
            pool.Add(entity);
            return true;
        }
        
#if ENABLE_IL2CPP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool TryGet<T>(this EcsPool<T> pool, int entity, ref T component) where T : struct
        {
            if (!pool.Has(entity)) 
                return false;
            
            component = ref pool.Get(entity);
            return true;
        }
        
#if ENABLE_IL2CPP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool TryRemove<T>(this EcsPool<T> pool, EcsPackedEntity packedEntity)
            where T : struct
        {
            var world = pool.GetWorld();
            if (!packedEntity.Unpack(world, out var entity))
                return false;
            
            return TryRemove<T>(pool, entity);
        }
        
#if ENABLE_IL2CPP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool TryRemove<T>(this EcsPool<T> pool, int entity) where T : struct
        {
            if (!pool.Has(entity))
                return false;
            
            pool.Del(entity);
            return true;
        }
        
#if ENABLE_IL2CPP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool TryAdd<T>(this EcsPool<T> pool, EcsPackedEntity packedEntity, ref T component) where T : struct
        {
            var world = pool.GetWorld();
            if (!packedEntity.Unpack(world, out var entity))
                return false;

            component = ref pool.Add(entity);
            return true;
        }
        
#if ENABLE_IL2CPP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ref TComponent GetOrAddComponent<TComponent>(this EcsPool<TComponent> pool, int entity)
            where TComponent : struct
        {
            ref var component = ref pool.Has(entity) 
                ? ref pool.Get(entity) 
                : ref pool.Add(entity);
            return ref component;
        }
    }
}