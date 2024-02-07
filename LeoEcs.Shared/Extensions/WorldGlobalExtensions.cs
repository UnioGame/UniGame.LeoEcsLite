namespace UniGame.LeoEcs.Shared.Extensions
{
    using System;
    using Leopotam.EcsLite;
    using UniModules.UniCore.Runtime.Extension;
    using UniModules.UniCore.Runtime.Utils;
    using UniModules.UniGame.Context.Runtime.Context;

    public static class WorldGlobalExtensions
    {
        private static MemorizeItem<EcsWorld, EntityContext> _globalValues = MemorizeTool
            .Memorize<EcsWorld, EntityContext>(x =>
            {
                var context = new EntityContext();
                var worldLifeTime = x.GetWorldLifeTime();
                context.AddTo(worldLifeTime);
                return context;
            });

        public static bool HasFlag<T>(this EcsWorld world, T flag)
            where T  : Enum
        {
            var context = _globalValues[world];
            if(!context.Contains<T>()) return false;
            
            var value = context.Get<T>();
            var isSet = value.IsFlagSet(flag);
            return isSet;
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
            globals.Publish(value);
            return value;
        }
    }
}