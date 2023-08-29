namespace UniGame.LeoEcs.Converter.Runtime
{
    using Core.Runtime;

    public interface IEcsEntity
    {
        public int Entity { get; }
        
        public ILifeTime LifeTime { get; }
    }
}