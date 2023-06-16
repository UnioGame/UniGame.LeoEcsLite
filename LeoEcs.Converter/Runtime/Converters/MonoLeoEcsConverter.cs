namespace UniGame.LeoEcs.Converter.Runtime.Converters
{
    using System.Threading;
    using Abstract;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UnityEngine.Serialization;

    public class MonoLeoEcsConverter<TConverter> : MonoBehaviour,ILeoEcsMonoComponentConverter
        where TConverter : ILeoEcsMonoComponentConverter
    {

        [FormerlySerializedAs("_converter")]
        [HideLabel]
        [SerializeField]
        [InlineProperty]
        public TConverter converter;

        public TConverter Converter => converter;
        
        public bool IsEnabled => converter.IsEnabled;
        
        public void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            if (converter == null) 
                return;
            
            converter.Apply(target, world, entity, cancellationToken);

            OnApply(gameObject, world, entity, cancellationToken);
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