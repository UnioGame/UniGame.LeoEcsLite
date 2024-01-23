namespace UniGame.LeoEcs.Converter.Runtime.Abstract
{
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;

    public interface IEcsComponentConverter : 
        ILeoEcsConverterStatus,
        ISearchFilterable
    {
        public string Name { get; }
        
        void Apply(EcsWorld world, int entity);
    }
}