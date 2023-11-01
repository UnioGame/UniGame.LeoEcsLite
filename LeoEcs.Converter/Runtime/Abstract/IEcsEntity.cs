namespace UniGame.LeoEcs.Converter.Runtime
{
    using Core.Runtime;
    using Leopotam.EcsLite;

    public interface IEcsEntity
    {
        public int Entity { get; }
        
        public EcsPackedEntity PackedEntity { get; }
        
        public ILifeTime LifeTime { get; }
    }
}