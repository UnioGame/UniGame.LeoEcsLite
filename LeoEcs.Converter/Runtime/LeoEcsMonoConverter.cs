namespace UniGame.LeoEcs.Converter.Runtime
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Components;
    using Core.Runtime;
    using Cysharp.Threading.Tasks;
    using Editor;
    using Leopotam.EcsLite;
    using Shared.Extensions;
    using Sirenix.OdinInspector;
    using UniModules.UniCore.Runtime.DataFlow;
    using UnityEngine;
    using UnityEngine.Serialization;

    public class LeoEcsMonoConverter : MonoBehaviour, 
        ILeoEcsMonoConverter,
        IEcsEntity
    {
        #region inspector data

        [FormerlySerializedAs("convertToParentEntity")]
        [BoxGroup("converter settings")]
        [Tooltip("try to get parent entity and add components to it")]
        public bool attachToParentEntity = false;
        
        [BoxGroup("converter settings")]
        [SerializeField] 
        public bool destroyEntityOnDisable = true;
        [BoxGroup("converter settings")]
        [SerializeField] 
        public bool createEntityOnEnabled = true;
        [BoxGroup("converter settings")]
        [SerializeField] 
        public bool createEntityOnStart = false;
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

        public ILifeTime LifeTime => _entityLifeTime;

        public int Entity => ecsEntityId;
        
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

        public async UniTask Convert()
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

            ecsEntityId = await GetTargetConvertEntity(world);
            
            if (ecsEntityId < 0)
            {
#if UNITY_EDITOR
                Debug.LogError($"Target entity is invalid for {_originalName}",gameObject);
#endif
                state = EntityState.Destroyed;
                return;
            }
            
            state = EntityState.Created;
            
            _entityId = world.PackEntity(ecsEntityId);
            _world = world;
            
            Convert(world,ecsEntityId);
            
            state = EntityState.Created;
        }

        public void Convert(EcsWorld world, int ecsEntity)
        {
#if UNITY_EDITOR
            gameObject.name = $"{_originalName}_ENT_{ecsEntityId}";
#endif
            
            _converters ??= new List<ILeoEcsComponentConverter>();
            _converters.Clear();
            
            GetComponents(_converters);
            
            _converters.AddRange(serializableConverters);
            
            world.ApplyEcsComponents(gameObject,ecsEntity, _converters, false, _entityLifeTime.CancellationToken);
            world.ApplyEcsComponents(ecsEntity, assetConverters, _entityLifeTime.CancellationToken);
            world.ApplyEcsComponents(ecsEntity, _dynamicComponentConverters, _entityLifeTime.CancellationToken);
            world.ApplyEcsComponents(gameObject, ecsEntity, _dynamicConverters, _entityLifeTime.CancellationToken);

            foreach (var callback in _dynamicCallbacks)
                callback(_entityId);

            _dynamicCallbacks.Clear();
            _dynamicComponentConverters.Clear();
            _dynamicConverters.Clear();

            world.GetOrAddComponent<ObjectConverterComponent>(ecsEntity);
        }
        
        #endregion
        
        
        #region private methods

        private async UniTask<int> GetTargetConvertEntity(EcsWorld world)
        {
            if (!attachToParentEntity) return world.NewEntity();

            var parent = transform.parent;
            if (parent == null) return -1;
            
            var ecsParent = parent.GetComponentInParent<IEcsEntity>();
            if (ecsParent == null) return world.NewEntity();
            
            if(ecsParent.Entity >= 0) return ecsParent.Entity;

            var token = _entityLifeTime.CancellationToken;
            var parentToken = ecsParent.LifeTime.CancellationToken;

            while (ecsParent.Entity < 0 && !token.IsCancellationRequested && !parentToken.IsCancellationRequested)
                await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);

            return ecsParent.Entity;
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
            _entityLifeTime.Terminate();
        }

        private void Awake()
        {
            _originalName = name;
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