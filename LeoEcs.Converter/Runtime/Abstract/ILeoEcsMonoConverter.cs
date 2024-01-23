﻿namespace UniGame.LeoEcs.Converter.Runtime
{
    using System.Collections.Generic;
    using Abstract;
    using Leopotam.EcsLite;

    public interface ILeoEcsMonoConverter : 
        IComponentConverterProvider,
        IEcsEntity,
        IConnectableToEntity
    {
        bool AutoCreate { get; }
        
        void Convert(EcsWorld world, int ecsEntity);

        void DestroyEntity();
    }

    public interface IConnectableToEntity
    {
        void ConnectEntity(EcsWorld world, int ecsEntity);
    }

    public interface IComponentConverterProvider
    {
        IReadOnlyList<IEcsComponentConverter> ComponentConverters { get; }
    }
}