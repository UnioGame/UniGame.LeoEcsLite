namespace UniGame.LeoEcs.Bootstrap.Runtime.Config
{
    using System;
    using Leopotam.EcsLite;

    [Serializable]
    public class EcsWorldConfiguration
    {
        public int Entities = 512;
        public int RecycledEntities = 512;
        public int Pools = 512;
        public int Filters = 512;
        public int PoolDenseSize = 512;
        public int PoolRecycledSize = 512;
        public int EntityComponentsSize = 8;

        public EcsWorld.Config Create()
        {
            var config = new EcsWorld.Config()
            {
                Entities = Entities,
                RecycledEntities = RecycledEntities,
                Pools = Pools,
                Filters = Filters,
                PoolDenseSize = PoolDenseSize,
                PoolRecycledSize = PoolRecycledSize,
                EntityComponentsSize = EntityComponentsSize,
            };

            return config;
        }
    }
}