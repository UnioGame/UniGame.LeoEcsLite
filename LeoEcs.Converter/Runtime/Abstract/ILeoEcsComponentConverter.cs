namespace UniGame.LeoEcs.Converter.Runtime.Abstract
{
    using System.Threading;
    using Leopotam.EcsLite;
    using UnityEngine;

    public interface ILeoEcsComponentConverter
    {
        void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default);
    }
}