namespace UniGame.LeoEcs.ViewSystem.Converters
{
    using System.Threading;
    using Converter.Runtime;
    using Converter.Runtime.Abstract;
    using Leopotam.EcsLite;
    using UiSystem.Runtime;
    using UniGame.ViewSystem.Runtime;
    using UnityEngine;

    [RequireComponent(typeof(LeoEcsMonoConverter))]
    public abstract class EcsUiView<TViewModel> : View<TViewModel>,
        ILeoEcsComponentConverter,
        IConverterEntityDestroyHandler,
        IEcsView
        where TViewModel : class, IViewModel
    {
        private EcsViewDataConverter<TViewModel> _dataConverter = new EcsViewDataConverter<TViewModel>();
        
        public void Apply(GameObject target, EcsWorld world, 
            int entity, CancellationToken cancellationToken = default)
        {
            _dataConverter.Apply(target,world,entity,cancellationToken);
            
            OnApply(world,entity);
        }
        
        public void OnEntityDestroy(EcsWorld world, int entity)
        {
            _dataConverter.OnEntityDestroy(world, entity);

            EntityDestroy(world, entity);
        }

        protected virtual void EntityDestroy(EcsWorld world, int entity){}
        
        protected virtual void OnApply(EcsWorld world, int entity){}

    }
}