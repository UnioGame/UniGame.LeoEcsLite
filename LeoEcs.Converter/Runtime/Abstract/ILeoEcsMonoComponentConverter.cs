using Sirenix.OdinInspector;

namespace UniGame.LeoEcs.Converter.Runtime.Abstract
{
    using System.Threading;
    using Leopotam.EcsLite;
    using UnityEngine;

    public interface ILeoEcsMonoComponentConverter 
        : ILeoEcsComponentConverter
        ,ILeoEcsConverterStatus { }
    
    public interface IComponentConverter : 
        IEcsComponentConverter,
        ILeoEcsConverterStatus,
        ISearchFilterable
    {
    }
    
    public interface IEcsComponentConverter
    {
        void Apply(EcsWorld world, int entity, CancellationToken cancellationToken = default);
    }

    public interface ILeoEcsComponentConverter
    {
        void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default);
    }
    
}