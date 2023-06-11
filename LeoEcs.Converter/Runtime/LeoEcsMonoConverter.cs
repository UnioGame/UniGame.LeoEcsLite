namespace UniGame.LeoEcs.Converter.Runtime
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Components;
    using Cysharp.Threading.Tasks;
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

        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [Space(8)]
        [InlineProperty]
        [SerializeReference]
        public List<ILeoEcsMonoComponentConverter> _serializableConverters = new List<ILeoEcsMonoComponentConverter>();

        [Space(8)] 
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)] 
        [InlineEditor()]
        public List<LeoEcsConverterAsset> assetConverters = new List<LeoEcsConverterAsset>();

        [Space] [ReadOnly] [BoxGroup("runtime info")] [ShowIf(nameof(IsRuntime))] [SerializeField]
        public int ecsEntityId = -1;

        #endregion

        #region private data

        private EcsPackedEntity _entityId;

        private EntityState _state = EntityState.Destroyed;

        private List<ILeoEcsComponentConverter> _converters = new List<ILeoEcsComponentConverter>();
        private List<ILeoEcsComponentConverter> _dynamicConverters = new List<ILeoEcsComponentConverter>();
        private List<IEcsComponentConverter> _dynamicComponentConverters = new List<IEcsComponentConverter>();
        private List<Action<EcsPackedEntity>> _dynamicCallbacks = new List<Action<EcsPackedEntity>>();
        private LifeTimeDefinition _entityLifeTime = new LifeTimeDefinition();

        private EcsWorld _world;

        #endregion

        public bool IsRuntime => Application.isPlaying;

        public bool IsPlayingAndReady => IsRuntime && ecsEntityId >= 0;

        public bool IsCreated => _state == EntityState.Created;

        public EcsPackedEntity EntityId => _entityId;

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
            _entityLifeTime ??= new LifeTimeDefinition();
            //get all converters
            _converters ??= new List<ILeoEcsComponentConverter>();
            _converters.AddRange(_serializableConverters);
            _converters.AddRange(GetComponents<ILeoEcsComponentConverter>());
        }

        #endregion

        #region private methods

        private async UniTask<EcsPackedEntity> Convert()
        {
            var world = await gameObject
                .WaitWorldReady(_entityLifeTime.CancellationToken);

            return _state == EntityState.Destroyed
                ? _entityId
                : Convert(world);
        }

        private EcsPackedEntity Convert(EcsWorld world)
        {
            if (IsCreated || world.IsAlive() == false)
                return _entityId;

            _world = world;

            ecsEntityId = gameObject.CreateEcsEntityFromGameObject(world,
                _converters, false,
                _entityLifeTime.CancellationToken);

            _entityId = world.PackEntity(ecsEntityId);
            _state = EntityState.Created;

            world.ApplyEcsComponents(ecsEntityId, assetConverters, _entityLifeTime.CancellationToken);
            world.ApplyEcsComponents(ecsEntityId, _dynamicComponentConverters, _entityLifeTime.CancellationToken);
            world.ApplyEcsComponents(gameObject, ecsEntityId, _dynamicConverters, _entityLifeTime.CancellationToken);

            foreach (var callback in _dynamicCallbacks)
                callback(_entityId);

            _dynamicCallbacks.Clear();
            _dynamicComponentConverters.Clear();
            _dynamicConverters.Clear();

            world.GetOrAddComponent<ObjectConverterComponent>(ecsEntityId);
            
            return _entityId;
        }

        private void CreateEntity()
        {
            if (_state == EntityState.Created)
                return;

            _state = EntityState.Creating;
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
            if (_state != EntityState.Created)
            {
                MarkAsDestroyed();
                return;
            }

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
            
            MarkAsDestroyed();
        }

        private void MarkAsDestroyed()
        {
            LeoEcsTool.DestroyEntity(_entityId, _world);
            
            ecsEntityId = -1;

            _state = EntityState.Destroyed;
            _entityId = new EcsPackedEntity();
            _entityLifeTime.Release();
        }

        #endregion

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            foreach (var converter in _serializableConverters)
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