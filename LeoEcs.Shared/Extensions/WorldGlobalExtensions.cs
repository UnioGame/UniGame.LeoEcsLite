namespace UniGame.LeoEcs.Shared.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Leopotam.EcsLite;
    using UniModules.UniCore.Runtime.Extension;
    using UniModules.UniCore.Runtime.Utils;
    using UniModules.UniGame.Context.Runtime.Context;

    public static class WorldGlobalExtensions
    {
        private static MemorizeItem<EcsWorld, WorldValueMap> _globalValues = MemorizeTool
            .Memorize<EcsWorld, WorldValueMap>(x =>
            {
                var context = new WorldValueMap();
                var worldLifeTime = x.GetWorldLifeTime();
                worldLifeTime.AddCleanUpAction(() => context.Clear());
                return context;
            });
        
        public static bool TryGetGlobal<T>(this EcsWorld world, out T value)
        {
            var globals = _globalValues[world];
            value = default;
            var targetType = typeof(T);

            if (!globals.TryGetValue(targetType, out var targetValue)) return 
                false;
            
            value = (T)targetValue;
            return true;
        }
        
        public static bool TryGetGlobal(this EcsWorld world,Type type, out object value)
        {
            var globals = _globalValues[world];
            return globals.TryGetValue(type, out value);
        }
        
        public static T GetGlobal<T>(this EcsWorld world)
        {
            var globals = _globalValues[world];
            var value = globals.Get<T>();
            return value;
        }
        
        public static T SetGlobal<T>(this EcsWorld world,T value)
        {
            var globals = _globalValues[world];
            globals.Set(value);
            return value;
        }
    }

    public class WorldValueMap : Dictionary<Type, object>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains<T>() => ContainsKey(typeof(T));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get<T>()
        {
            if(TryGetValue(typeof(T),out var value))
                return (T)value;
            return default;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set<T>(T value) => this[typeof(T)] = value;
    }
}