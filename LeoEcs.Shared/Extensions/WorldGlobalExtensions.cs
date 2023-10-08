namespace UniGame.LeoEcs.Shared.Extensions
{
    using Leopotam.EcsLite;
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