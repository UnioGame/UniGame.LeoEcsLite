namespace UniGame.LeoEcs.Bootstrap.Runtime.Abstract
{
    using Leopotam.EcsLite;

    public interface IEcsPostInitializeAction
    {
        public void Apply(IEcsSystems ecsSystems,IEcsSystem system);
    }
}