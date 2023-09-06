namespace UniGame.LeoEcs.Converter.Runtime
{
    using System.Collections.Generic;
    using Abstract;
    using Leopotam.EcsLite;

    public interface ILeoEcsMonoConverter : IMonoConverterProvider,
        IComponentConverterProvider,
        IEcsEntity,
        IConnectableToEntity
    {
        void Convert(EcsWorld world, int ecsEntity);

        void DestroyEntity();
    }

    public interface IConnectableToEntity
    {
        void ConnectEntity(EcsWorld world, int ecsEntity);
    }
    
    public interface IMonoConverterProvider
    {
        IReadOnlyList<ILeoEcsComponentConverter> MonoConverters { get; }
    }

    public interface IComponentConverterProvider
    {
        IReadOnlyList<IEcsComponentConverter> ComponentConverters { get; }
    }
}