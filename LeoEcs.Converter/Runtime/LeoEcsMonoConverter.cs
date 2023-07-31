namespace UniGame.LeoEcs.Converter.Runtime
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Components;
    using Cysharp.Threading.Tasks;
    using Editor;
    using Leopotam.EcsLite;
    using Shared.Extensions;
    using Sirenix.OdinInspector;
    using UniModules.UniCore.Runtime.DataFlow;
    using UnityEngine;
    using UnityEngine.Serialization;

    public class LeoEcsMonoConverter : MonoBehaviour, ILeoEcsMonoConverter
    {
        #region inspector data

        [SerializeField] public bool destroyEntityOnDisable = true;
        [SerializeField] public bool createEntityOnEnabled = true;
        [SerializeField] public bool createEntityOnStart = false;
        [SerializeField] public bool destroyOnDestroy = false;
        
        [FormerlySerializedAs("_serializableConverters")]
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [Space(8)]
        [InlineProperty]
        [SerializeReference]
        public List<ILeoEcsMonoComponentConverter> serializableConverters = new List<ILeoEcsMonoComponentConverter>();

        [Space(8)] 
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)] 
        [InlineEditor()]
        public List<LeoEcsConverterAsset> assetConverters = new List<LeoEcsConverterAsset>();

        [Space] [ReadOnly] 
        [BoxGroup("runtime info")] 
        [ShowIf(nameof(IsRuntime))] 
        [SerializeField]
        public int ecsEntityId = -1;

        [BoxGroup("runtime info")] 
        [ReadOnly] 
        public EntityState state = EntityState.Destroyed;
        
        #endregion

        #region private data

        private EcsPackedEntity _entityId;
        
        private List<ILeoEcsComponentConverter> _converters = new List<ILeoEcsComponentConverter>();
        private List<ILeoEcsComponentConverter> _dynamicConverters = new List<ILeoEcsComponentConverter>();
        private List<IEcsComponentConverter> _dynamicComponentConverters = new List<IEcsComponentConverter>();
        private List<Action<EcsPackedEntity>> _dynamicCallbacks = new List<Action<EcsPackedEntity>>();
        private LifeTimeDefinition _entityLifeTime = new LifeTimeDefinition();

        private EcsWorld _world;
        private string _originalName;

        #endregion

        public bool IsRuntime => Application.isPlaying;

        public bool IsPlayingAndReady => IsRuntime && ecsEntityId >= 0;

        public bool IsCreated => state == EntityState.Created;

        public EcsPackedEntity EntityId => _entityId;
        
        public EcsWorld World => _world;

        #region public methods

        public void RegisterDynamicConverter(ILeoEcsComponentConverter converter)
        {
            if (ecsEntityId < 0)
            {
                _dynamicConverters.Add(converter);
                return;
            }

            converter.Apply(gameObject, _world, ecsEntityId, _entityLifeTime.CancellationToken);
        }

        public void RegisterDynamicConverter(IEcsComponentConverter converter)
        {
            if (ecsEntityId < 0)
            {
                _dynamicComponentConverters.Add(converter);
                return;
            }

            converter.Apply(_world, ecsEntityId, _entityLifeTime.CancellationToken);
        }

        public void RegisterDynamicCallback(Action<EcsPackedEntity> converter)
        {
            if (ecsEntityId < 0)
            {
                _dynamicCallbacks.Add(converter);
                return;
            }

            converter(_entityId);
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
            if (!destroyOnDestroy)
                return;
            DestroyEntity();
        }

        private void Awake()
        {
            _originalName = name;
            _entityLifeTime ??= new LifeTimeDefinition();
            //get all converters
            _converters ??= new List<ILeoEcsComponentConverter>();
        }

        #endregion

        #region private methods

        private async UniTask Convert()
        {
            if(IsCreated) return;
            
            var world = LeoEcsConvertersData.World ?? 
                        await gameObject.WaitWorldReady(_entityLifeTime.CancellationToken);

            if (this == null) return;
            
            if (world.IsAlive() == false)
            {
                state = EntityState.Destroyed;
                return;
            }
            
            if (state == EntityState.Destroyed)
            {
                DestroyEntity();
                return;
            }

            ecsEntityId = world.NewEntity();
            state = EntityState.Created;
            _entityId = world.PackEntity(ecsEntityId);
            
            Convert(world,ecsEntityId);
        }

        private void Convert(EcsWorld world, int ecsEntity)
        {
            _world = world;
            _converters ??= new List<ILeoEcsComponentConverter>();
            _converters.Clear();
            
            GetComponents(_converters);
            
            _converters.AddRange(serializableConverters);
            
            gameObject.CreateEcsEntityFromGameObject(
                ecsEntity,
                world,
                _converters,
                false,
                _entityLifeTime.CancellationToken);

            
            state = EntityState.Created;

#if UNITY_EDITOR
            gameObject.name = $"{_originalName}_ENT_{ecsEntityId}";
#endif
            
            world.ApplyEcsComponents(ecsEntityId, assetConverters, _entityLifeTime.CancellationToken);
            world.ApplyEcsComponents(ecsEntityId, _dynamicComponentConverters, _entityLifeTime.CancellationToken);
            world.ApplyEcsComponents(gameObject, ecsEntityId, _dynamicConverters, _entityLifeTime.CancellationToken);

            foreach (var callback in _dynamicCallbacks)
                callback(_entityId);

            _dynamicCallbacks.Clear();
            _dynamicComponentConverters.Clear();
            _dynamicConverters.Clear();

            world.GetOrAddComponent<ObjectConverterComponent>(ecsEntityId);
        }

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
            DestroyEntity(_entityId);
        }

        private void DestroyEntity(EcsPackedEntity entity)
        {
            //mark state as destroyed
            state = EntityState.Destroyed;
            
            //if entity already created when destroy immediate
            if (ecsEntityId < 0) return;
            
            if (_world == null || _world.IsAlive() == false) return;

            if (!entity.Unpack(_world, out var targetEntity))
                return;
            
            //notify converters about destroy
            foreach (var converter in _converters)
            {
                if (converter is IConverterEntityDestroyHandler destroyHandler)
                    destroyHandler.OnEntityDestroy(_world, ecsEntityId);
            }

            //notify converters about destroy
            foreach (var converter in assetConverters)
            {
                if (converter is not IConverterEntityDestroyHandler destroyHandler)
                    continue;
                destroyHandler.OnEntityDestroy(_world, ecsEntityId);
            }

            //clean up converter entity data
            LeoEcsTool.DestroyEntity(targetEntity, _world);
                
            ecsEntityId = -1;

            _entityId = new EcsPackedEntity();
            _entityLifeTime.Release();
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
            EntityEditorCommands.OpenEntityInfo(ecsEntityId);
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