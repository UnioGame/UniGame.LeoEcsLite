namespace UniGame.LeoEcs.Bootstrap.Runtime
{
    using Leopotam.EcsLite;

    public interface IEcsAspect
    {
        public void Initialize(EcsWorld world);
    }
}