namespace UniGame.LeoEcs.Converter.Runtime
{
    using Leopotam.EcsLite;

    public interface ILeoEcsMonoConverter
    {
        EcsPackedEntity Convert(EcsWorld world);
    }
}