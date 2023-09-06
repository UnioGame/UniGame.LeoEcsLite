namespace UniGame.LeoEcs.Converter.Runtime.Converters
{
    using System.Threading;
    using Abstract;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UnityEngine.Serialization;

    [RequireComponent(typeof(LeoEcsMonoConverter))]
    public class MonoLeoEcsConverter<TConverter> : MonoBehaviour,
        ILeoEcsMonoComponentConverter,
        IConverterEntityDestroyHandler
        where TConverter : ILeoEcsMonoComponentConverter
    {
        #region inspector
        
        [FormerlySerializedAs("_converter")]
        [HideLabel]
        [SerializeField]
        [InlineProperty]
        public TConverter converter;

        public TConverter Converter => converter;
        
        public bool IsEnabled => converter.IsEnabled;

        public bool IsRuntime => Application.isPlaying;
        
        public string Name => converter == null ? "EMPTY" : converter.Name;
        
        #endregion

        public EcsPackedEntity Entity{get; private set;}
        protected EcsWorld World{get; private set;}
        
        public void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            if (converter == null) 
                return;
            
            converter.Apply(target, world, entity, cancellationToken);

            OnApply(gameObject, world, entity, cancellationToken);

            Entity = world.PackEntity(entity);
            World = world;
        }
        
        public virtual void OnEntityDestroy(EcsWorld world, int entity)
        {
            if(converter is IConverterEntityDestroyHandler destroyHandler)
                destroyHandler.OnEntityDestroy(world, entity);
        }

        protected virtual void OnApply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            
        }
        
        private void OnDrawGizmos()
        {
            if (converter is ILeoEcsGizmosDrawer gizmosDrawer)
                gizmosDrawer.DrawGizmos(gameObject);
        }
        
    }
}