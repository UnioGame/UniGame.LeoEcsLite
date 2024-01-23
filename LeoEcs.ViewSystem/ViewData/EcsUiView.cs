namespace UniGame.LeoEcs.ViewSystem.Converters
{
    using Converter.Runtime;
    using Converter.Runtime.Abstract;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;
    using UiSystem.Runtime;
    using UniGame.ViewSystem.Runtime;
    using UnityEngine;

    [RequireComponent(typeof(LeoEcsMonoConverter))]
    public abstract class EcsUiView<TViewModel> : View<TViewModel>,
        IEcsComponentConverter,
        IConverterEntityDestroyHandler,
        IEcsView
        where TViewModel : class, IViewModel
    {
        public bool isEnabled = true;
        
        [PropertySpace(8)]
        [FoldoutGroup("settings")]
        [InlineProperty]
        [HideLabel]
        public EcsViewSettings settings = new();
        
        private EcsViewDataConverter<TViewModel> _dataConverter = new();

        public virtual bool IsEnabled => isEnabled;

        public virtual string Name => GetType().Name;

        public void Apply(EcsWorld world, int entity)
        {
            _dataConverter.SetUp(settings);
            _dataConverter.Apply(world,entity);
            
            OnApply(world,entity);
        }
        
        public void OnEntityDestroy(EcsWorld world, int entity)
        {
            _dataConverter.OnEntityDestroy(world, entity);

            EntityDestroy(world, entity);
        }

        protected virtual void EntityDestroy(EcsWorld world, int entity){}
        
        protected virtual void OnApply(EcsWorld world, int entity){}
        
        public bool IsMatch(string searchString)
        {
            throw new System.NotImplementedException();
        }
    }
}