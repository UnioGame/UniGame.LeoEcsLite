namespace UniGame.LeoEcs.Converter.Runtime.Abstract
{
    using Leopotam.EcsLite;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

    
    public interface IEcsComponentConverter : 
        ILeoEcsConverterStatus
#if ODIN_INSPECTOR
        , ISearchFilterable
#endif
    {
        public string Name { get; }
        
        void Apply(EcsWorld world, int entity);
    }
}