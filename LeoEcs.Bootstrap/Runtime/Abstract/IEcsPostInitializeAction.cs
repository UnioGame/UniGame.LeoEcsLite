namespace UniGame.LeoEcs.Bootstrap.Runtime.Abstract
{
    using Core.Runtime;
    using Leopotam.EcsLite;

    public interface IEcsPostInitializeAction
    {
        public (IEcsSystems value, bool replace) Apply(IEcsSystems ecsSystems,IContext context);
    }
}