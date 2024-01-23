namespace UniGame.LeoEcs.Converter.Runtime
{
    using System;
    using System.Collections.Generic;
    using Abstract;

    public interface IEcsConverterProvider
    {
        IEnumerable<IEcsComponentConverter> Converters { get; }
        
        T GetConverter<T>() where T : class;

        void RemoveConverter<T>() where T : IEcsComponentConverter;
        
        IEcsComponentConverter GetConverter(Type target);
    }
}