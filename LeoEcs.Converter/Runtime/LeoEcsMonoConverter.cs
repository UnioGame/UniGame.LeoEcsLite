namespace UniGame.LeoEcs.Converter.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Abstract;
    using Core.Runtime;
    using Cysharp.Threading.Tasks;
    using Editor;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;
    using UniModules.UniCore.Runtime.DataFlow;
    using UnityEngine;
    using UnityEngine.Serialization;

    public class LeoEcsMonoConverter : 
        MonoBehaviour, 
        ILeoEcsMonoConverter,
        IEcsEntity
    {
        #region inspector data
        
        [BoxGroup("converter settings")]
        [SerializeField] 
        public bool createEntityOnEnabled = true;
        
        [BoxGroup("converter settings")]
        [SerializeField] 
        public bool createEntityOnStart = false;

        [Space]
        [BoxGroup("converter settings")]
        [SerializeField] 
        public bool destroyEntityOnDisable = true;
        
        [BoxGroup("converter settings")]
        [SerializeField] 
        public bool destroyOnDestroy = false;
        
        [FormerlySerializedAs("_serializableConverters")]
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [Space(8)]
        [InlineProperty]
        [SerializeReference]
        public List<ILeoEcsMonoComponentConverter> serializableConverters = new List<ILeoEcsMonoComponentConverter>();

        [Space(8)] 
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)] 
        [InlineEditor()]
        [ListDrawerSettings(ListElementLabelName = "@Name")]
        public List<LeoEcsConverterAsset> assetConverters = new List<LeoEcsConverterAsset>();

        [FormerlySerializedAs("ecsEntityId")]
        [Space]
        [ReadOnly]
        [BoxGroup("runtime info")] 
        [ShowIf(nameof(IsRuntime))] 
        [SerializeField]
        public int entity = -1;

        [BoxGroup("runtime info")] 
        [ReadOnly] 
        public EntityState state = EntityState.Destroyed;
        
#endregion

#region private data

        private EcsWorld _world;
        private EcsPackedEntity _entityId;
        private List<ILeoEcsComponentConverter> _converters = new List<ILeoEcsComponentConverter>();
        private LifeTimeDefinition _entityLifeTime = new LifeTimeDefinition();
        
        #endregion

        public bool IsRuntime => Application.isPlaying;

        public bool IsPlayingAndReady => IsRuntime && entity >= 0;

        public bool IsCreated => state == EntityState.Created;

        public EcsPackedEntity EntityId => _entityId;
        
        public EcsWorld World => _world;

        public ILifeTime LifeTime => _entityLifeTime;

        public int Entity => entity;

        public IReadOnlyList<ILeoEcsComponentConverter> MonoConverters => UpdateMonoConverter();

        public IReadOnlyList<IEcsComponentConverter> ComponentConverters => assetConverters;
        
        #region public methods
        
        public async UniTask Convert()
        {
            if(IsCreated) return;
            
            var world = LeoEcsConvertersData.World ?? 
                        await gameObject.WaitWorldReady(_entityLifeTime.CancellationToken);

            if (this == null) return;
            
            if (world.IsAlive() == false)
            {
                state = EntityState.Destroyed;
                entity = -1;
                return;
            }
            
            if (state == EntityState.Destroyed)
            {
                DestroyEntity();
                return;
            }
            
            if (state == EntityState.Created) return;
            
            var targetEntity = world.NewEntity();
            
            Convert(world, targetEntity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ConnectEntity(EcsWorld world,int ecsEntity)
        {
            _world = world;
            entity = ecsEntity;
            state = EntityState.Created;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Convert(EcsWorld world, int ecsEntity)
        {
            ConnectEntity(world,ecsEntity);
            gameObject.ConvertGameObjectToEntity(world,ecsEntity);
        }
        
#endregion
        
#region private methods

        private List<ILeoEcsComponentConverter> UpdateMonoConverter()
        {
            if(_converters.Count > 0) return _converters;
            
            _converters ??= new List<ILeoEcsComponentConverter>();
            _converters.Clear();
            
            GetComponents(_converters);
            
            _converters.AddRange(serializableConverters);
            return _converters;
        }
        
        
        private void CreateEntity()
        {
            if (IsCreated) return;

            state = EntityState.Creating;
            _entityLifeTime.Release();

            Convert().AttachExternalCancellation(_entityLifeTime.CancellationToken)
                .Forget();
        }

        [ShowIf(nameof(IsPlayingAndReady))]
        [Button("Destroy")]
        private void DestroyObject()
        {
            DestroyEntity();
            Destroy(gameObject);
        }

        public void DestroyEntity()
        {
            DestroyEntity(_entityId);
        }

        private void DestroyEntity(EcsPackedEntity entity)
        {
            //mark state as destroyed
            state = EntityState.Destroyed;
            
            //if entity already created when destroy immediate
            if (this.entity < 0) return;
            
            if (_world == null || _world.IsAlive() == false) return;

            if (!entity.Unpack(_world, out var targetEntity))
                return;
            
            //notify converters about destroy
            foreach (var converter in _converters)
            {
                if (converter is IConverterEntityDestroyHandler destroyHandler)
                    destroyHandler.OnEntityDestroy(_world, this.entity);
            }

            //notify converters about destroy
            foreach (var converter in assetConverters)
            {
                if (converter is not IConverterEntityDestroyHandler destroyHandler) continue;
                destroyHandler.OnEntityDestroy(_world, this.entity);
            }

            //clean up converter entity data
            LeoEcsTool.DestroyEntity(targetEntity, _world);
                
            this.entity = -1;
            
            _entityId = new EcsPackedEntity();
            _entityLifeTime.Release();
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
            if (!destroyEntityOnDisable) return;
            DestroyEntity();
        }

        private void OnDestroy()
        {
            if (destroyOnDestroy) 
                DestroyEntity();
            _entityLifeTime.Terminate();
        }

        private void Awake()
        {
            _entityLifeTime ??= new LifeTimeDefinition();
            //get all converters
            _converters ??= new List<ILeoEcsComponentConverter>();
        }

#endregion
        
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            foreach (var converter in serializableConverters)
            {
                if (converter is ILeoEcsGizmosDrawer gizmosDrawer)
                    gizmosDrawer.DrawGizmos(gameObject);
            }

            foreach (var converter in assetConverters)
            {
                var converters = converter.converters;
                foreach (var converterValue in converters)
                {
                    if (converterValue.Value is ILeoEcsGizmosDrawer gizmosDrawer)
                        gizmosDrawer.DrawGizmos(gameObject);
                }
            }
        }

        [BoxGroup("runtime info")] 
        [EnableIf(nameof(IsPlayingAndReady))]
        [GUIColor(1f,0.5f,0.1f)]
        [Button(ButtonSizes.Large,Icon = SdfIconType.Book)]
        public void ShowComponents()
        {
            EntityEditorCommands.OpenEntityInfo(entity);
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