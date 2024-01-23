namespace UniGame.LeoEcs.Converter.Runtime.Converters
{
    using Abstract;
    using Leopotam.EcsLite;
    using Shared.Components;
    using Shared.Extensions;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UnityEngine.Serialization;

    [RequireComponent(typeof(LeoEcsMonoConverter))]
    public class MonoLeoEcsConverter<TConverter> : 
        MonoBehaviour,
        IEcsComponentConverter,
        IConverterEntityDestroyHandler
        where TConverter : IEcsComponentConverter
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
        public void Apply(EcsWorld world, int entity)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        public EcsPackedEntity Entity{get; private set;}
        protected EcsWorld World{get; private set;}
        
        public void Apply(GameObject target, EcsWorld world, int entity)
        {
            if (converter == null) return;
            
            ref var gameObjectComponent = ref world
                .GetOrAddComponent<GameObjectComponent>(entity);
            gameObjectComponent.Value = target;
            
            converter.Apply(world, entity);

            OnApply(gameObject, world, entity);

            Entity = world.PackEntity(entity);
            World = world;
        }
        
        public virtual void OnEntityDestroy(EcsWorld world, int entity)
        {
            if(converter is IConverterEntityDestroyHandler destroyHandler)
                destroyHandler.OnEntityDestroy(world, entity);
        }

        protected virtual void OnApply(GameObject target, EcsWorld world, int entity)
        {
            
        }
        
        private void OnDrawGizmos()
        {
            if (converter is ILeoEcsGizmosDrawer gizmosDrawer)
                gizmosDrawer.DrawGizmos(gameObject);
        }

        public bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return true;
            if (searchString.Contains(name)) return true;
            if (converter.IsMatch(searchString)) return true;
            
            return false;
        }
    }
}