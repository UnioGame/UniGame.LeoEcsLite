namespace UniGame.LeoEcs.Converter.Runtime
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;
    using UniModules.UniCore.Runtime.DataFlow;
    using UnityEngine;

    public class LeoEcsMonoConverter : MonoBehaviour, ILeoEcsMonoConverter
    {
        #region inspector data

        public EntityState state = EntityState.Destroyed;
        
        [SerializeField]
        public bool destroyEntityOnDisable = true;
        [SerializeField]
        public bool createEntityOnEnabled = true;
        [SerializeField]
        public bool createEntityOnStart = false;
        [SerializeField]
        public bool destroyEntityOnDestroy = true;

        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [Space(8)]
        [InlineProperty]
        [SerializeReference]
        public List<ILeoEcsMonoComponentConverter> _serializableConverters = new List<ILeoEcsMonoComponentConverter>();

        [Space(8)]
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [InlineEditor()]
        public List<LeoEcsConverterAsset> assetConverters = new List<LeoEcsConverterAsset>();

        [Space] 
        [ReadOnly] 
        [BoxGroup("runtime info")] 
        [ShowIf(nameof(IsRuntime))] 
        [SerializeField]
        public int ecsEntityId = -1;
        
        private EcsPackedEntity _entityId;

        #endregion

        #region private data

        private List<ILeoEcsComponentConverter> _converters = new List<ILeoEcsComponentConverter>();

        private EcsWorld _world;

        private LifeTimeDefinition _entityLifeTime;

        #endregion

        public bool IsRuntime => Application.isPlaying;

        public bool IsPlayingAndReady => IsRuntime && state == EntityState.Created;

        public bool IsCreated => state == EntityState.Created;
        
        public EcsPackedEntity EntityId => _entityId;

        #region public methods
        
        public async UniTask<EcsPackedEntity> Convert()
        {
            var world = await gameObject
                .WaitWorldReady(_entityLifeTime.CancellationToken);

            if (state == EntityState.Destroyed)
                return _entityId;
            
            return Convert(world);
        }

        public EcsPackedEntity Convert(EcsWorld world)
        {
            if (IsCreated || world.IsAlive() == false) 
                return _entityId;
            
            state = EntityState.Created;
            _world = world;
            
            ecsEntityId = gameObject.CreateEcsEntityFromGameObject(world,
                    _converters, false, 
                    _entityLifeTime.CancellationToken);

            _entityId = world.PackEntity(ecsEntityId);
            
            world.ApplyEcsComponents(ecsEntityId,assetConverters,_entityLifeTime.CancellationToken);
            
            return _entityId;
        }
        
        #endregion

        #region unity methods

        // Start is called before the first frame update
        private void Start()
        {
            if (!createEntityOnStart)
                return;
            CreateEntity();
        }

        private void OnEnable()
        {
            if (!createEntityOnEnabled)
                return;
            CreateEntity();
        }
        
        private void OnDisable()
        {
            if (!destroyEntityOnDisable)
                return;
            DestroyEntity();
        }
        
        private void OnDestroy()
        {
            if (destroyEntityOnDestroy)
            {
                DestroyEntity();
            }
        }

        private void Awake()
        {
            _entityLifeTime = new LifeTimeDefinition();
            //get all converters
            _converters ??= new List<ILeoEcsComponentConverter>();
            _converters.AddRange(_serializableConverters);
            _converters.AddRange(GetComponents<ILeoEcsComponentConverter>());
        }
        
        #endregion

        #region private methods

        private void CreateEntity()
        {
            if (state == EntityState.Created) 
                return;
            
            state = EntityState.Creating;
            _entityLifeTime.Release();
            
            Convert().Forget();
        }

        [ShowIf(nameof(IsPlayingAndReady))]
        [Button("Destroy")]
        private void DestroyObject()
        {
            DestroyEntity();
            Destroy(gameObject);
        }
        
        private void DestroyEntity()
        {
            if (state != EntityState.Created)
            {
                MarkAsDestroyed();
                return;
            }

            //notify converters about destroy
            foreach (var converter in _converters)
            {
                if(converter is IConverterEntityDestroyHandler destroyHandler)
                    destroyHandler.OnEntityDestroy(_world,ecsEntityId);
            }
            
            //notify converters about destroy
            foreach (var converter in assetConverters)
            {
                if(converter is IConverterEntityDestroyHandler destroyHandler)
                    destroyHandler.OnEntityDestroy(_world,ecsEntityId);
            }
            
            state = EntityState.Destroyed;
            
            ecsEntityId = -1;
            
            LeoEcsTool.DestroyEntity(_entityId, _world);
        }

        private void MarkAsDestroyed()
        {
            state = EntityState.Destroyed;
            ecsEntityId = -1;
            _entityId = new EcsPackedEntity();
            _entityLifeTime.Release();
        }

        #endregion
        
#if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {
            foreach (var converter in _serializableConverters)
            {
                if(converter is ILeoEcsGizmosDrawer gizmosDrawer)
                    gizmosDrawer.DrawGizmos(gameObject);
            }
            foreach (var converter in assetConverters)
            {
                foreach (var converterValue in converter.converters)
                {
                    if(converterValue.Value is ILeoEcsGizmosDrawer gizmosDrawer)
                        gizmosDrawer.DrawGizmos(gameObject);
                }
            }
        }

#endif
    }

    [Serializable]
    public enum EntityState : byte
    {
        Creating,
        Created,
        Destroyed,
    }
}