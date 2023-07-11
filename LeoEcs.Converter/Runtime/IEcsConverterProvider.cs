namespace UniGame.LeoEcs.Converter.Runtime
{
    using System;
    using Abstract;

    public interface IEcsConverterProvider
    {
        T GetConverter<T>() where T : class;

        void RemoveConverter<T>() where T : IComponentConverter;
        
        IComponentConverter GetConverter(Type target);
    }
}