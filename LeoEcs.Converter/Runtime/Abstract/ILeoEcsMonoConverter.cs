namespace UniGame.LeoEcs.Converter.Runtime
{
    using System;
    using Abstract;
    using Leopotam.EcsLite;

    public interface ILeoEcsMonoConverter
    {
        void RegisterDynamicCallback(Action<EcsPackedEntity> converterAction);
        
        void RegisterDynamicConverter(ILeoEcsComponentConverter converter);
        
        void RegisterDynamicConverter(IEcsComponentConverter converter);
    }
}