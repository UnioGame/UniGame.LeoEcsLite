namespace UniGame.LeoEcs.Converter.Runtime.Abstract
{
    using System.Threading;
    using Leopotam.EcsLite;

    public interface IEcsComponentConverter
    {
        void Apply(EcsWorld world, int entity, CancellationToken cancellationToken = default);
    }
}