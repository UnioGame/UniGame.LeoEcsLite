using Sirenix.OdinInspector;

namespace UniGame.LeoEcs.Converter.Runtime.Abstract
{
    public interface ILeoEcsMonoComponentConverter
        : ILeoEcsComponentConverter, ILeoEcsConverterStatus
    {
        string Name { get; }
    }
    
    public interface IComponentConverter : 
        IEcsComponentConverter,
        ILeoEcsConverterStatus,
        ISearchFilterable
    {
    }
}