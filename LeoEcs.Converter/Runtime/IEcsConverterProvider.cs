namespace UniGame.LeoEcs.Converter.Runtime
{
    using System;
    using Abstract;

    public interface IEcsConverterProvider
    {
        T GetConverter<T>() where T : class;

        IComponentConverter GetConverter(Type target);
    }
}