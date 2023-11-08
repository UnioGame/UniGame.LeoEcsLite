namespace UniGame.LeoEcs.ViewSystem.Converters
{
    using System.Threading;
    using Converter.Runtime;
    using Converter.Runtime.Abstract;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;
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
        [PropertySpace(8)]
        [FoldoutGroup("ecs settings")]
        [InlineProperty]
        [HideLabel]
        public EcsViewSettings settings = new();
        
        private EcsViewDataConverter<TViewModel> _dataConverter = new();
        
        public void Apply(GameObject target, EcsWorld world, 
            int entity, CancellationToken cancellationToken = default)
        {
            _dataConverter.SetUp(settings);
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