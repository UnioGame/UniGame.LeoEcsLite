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

#endregion

#region private data

        private EntityState _state = EntityState.Destroyed;
        private EcsPackedEntity _packedEntity;
        private EcsWorld _world;
        private List<ILeoEcsComponentConverter> _converters = new List<ILeoEcsComponentConverter>();
        private LifeTimeDefinition _entityLifeTime = new LifeTimeDefinition();
        private int _generation;
        
#endregion

        public bool IsRuntime => Application.isPlaying;

        public bool IsAutoGenerating => createEntityOnEnabled || createEntityOnStart;

        public bool IsPlayingAndReady => IsRuntime && entity >= 0;

        public bool IsCreated => _state == EntityState.Created;

        public EcsWorld World => _world;

        public EcsPackedEntity PackedEntity => _packedEntity;
        
        public ILifeTime LifeTime => _entityLifeTime;

        public int Entity => entity;

        public bool AutoCreate => createEntityOnEnabled || createEntityOnStart;
        
        public IReadOnlyList<ILeoEcsComponentConverter> MonoConverters => UpdateMonoConverter();

        public IReadOnlyList<IEcsComponentConverter> ComponentConverters => assetConverters;
        
        #region public methods
        
        public async UniTask Convert()
        {
            _generation++; 
            var generation = _generation;

            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
            
            if(_state != EntityState.Creating) return;

            var world = LeoEcsGlobalData.World ?? 
                        await gameObject.WaitWorldReady(_entityLifeTime.Token);

            if (world.IsAlive() == false)   
            {
                DestroyEntity();
                return;
            }
            
            if(this == null ||
               _state != EntityState.Creating || 
               generation != _generation) return;
            
            var targetEntity = world.NewEntity();
            Convert(world, targetEntity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ConnectEntity(EcsWorld world,int ecsEntity)
        {
            entity = ecsEntity;
            
            _world = world;
            _packedEntity = world.PackEntity(ecsEntity);
            _state = EntityState.Created;
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
            if (_state != EntityState.Destroyed) return;

            _state = EntityState.Creating;
            _entityLifeTime.Release();

            Convert().AttachExternalCancellation(_entityLifeTime.Token)
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
            DestroyEcsEntity();
            SetDestroyedState();
        }

        private void DestroyEcsEntity()
        {
            if (_world == null || _world.IsAlive() == false) return;
            if (entity < 0) return;
            if (!_packedEntity.Unpack(_world, out var targetEntity)) return;
            
            //notify converters about destroy
            foreach (var converter in _converters)
            {
                if (converter is IConverterEntityDestroyHandler destroyHandler)
                    destroyHandler.OnEntityDestroy(_world, targetEntity);
            }
            //notify converters about destroy
            foreach (var converter in assetConverters)
            {
                if (converter is not IConverterEntityDestroyHandler destroyHandler) continue;
                destroyHandler.OnEntityDestroy(_world, targetEntity);
            }
            _world.DelEntity(targetEntity);
        }

        private void SetDestroyedState()
        {
            entity = -1;
            _state = EntityState.Destroyed;
            _packedEntity = default;
            _entityLifeTime.Release();
        }

#endregion
        
#region unity methods

        // Start is called before the first frame update
        private void Start()
        {
            if (!createEntityOnStart) return;
            CreateEntity();
        }

        private void OnEnable()
        {
            if (!createEntityOnEnabled) return;
            var frame = Time.frameCount;
            CreateEntity();
        }

        private void OnDisable()
        {
            if (!destroyEntityOnDisable) return;
            var frame = Time.frameCount;
            DestroyEntity();
        }

        private void OnDestroy()
        {
            if (destroyOnDestroy) DestroyEntity();
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
                if(converter == null) continue;
                
                var converters = converter.converters;
                foreach (var converterValue in converters)
                {
                    switch (converterValue.Value)
                    {
                        case null:
                            continue;
                        case ILeoEcsGizmosDrawer gizmosDrawer:
                            gizmosDrawer.DrawGizmos(gameObject);
                            break;
                    }
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