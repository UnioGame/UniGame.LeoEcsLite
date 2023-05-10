namespace UniGame.LeoEcs.Debug.Editor
{
    using System;
    using System.Collections.Generic;
    using Leopotam.EcsLite;
    using Runtime.ObjectPool.Extensions;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [Serializable]
    public class EntitiesEditorView
    {
        #region static data

        private static Color _oddColor = new Color(0.3f, 0.5f, 0.4f);
        private static Color _rowColor = new Color(0.3f, 0.6f, 0.6f);
        
        public static bool IsInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public void ResetStaticData()
        {
            IsInitialized = false;
        }
        
        #endregion
        
        #region inspector

        [HorizontalGroup()]
        [LabelWidth(60)]
        [LabelText("status :")]
        [GUIColor(nameof(GetStatusColor))]
        public string status;

        [HorizontalGroup()]
        [LabelWidth(60)]
        [LabelText("info :")]
        public string message;

        [Space(8)]
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)] 
        [InlineProperty]
        [ListDrawerSettings(DraggableItems = false,HideAddButton = true,
            HideRemoveButton = true,
            ElementColor = nameof(GetElementColor))]
        public List<EntityEditorView> entities = new List<EntityEditorView>();

        #endregion

        private EcsWorld _world;
        private HashSet<int> _uniqueEntities = new HashSet<int>();
        private EcsFilterData _cachedFilter = EcsFilterData.NoneFilterData;
        private EcsEditorFilter _filter = new EcsEditorFilter();
        private EditorEntityViewBuilder _viewBuilder = new EditorEntityViewBuilder();

        public void Initialize(EcsWorld world)
        {
            _world = world;

            if (_world == null || !VerifyView()) return;
            
            _viewBuilder.Initialize(_world);
            
            IsInitialized = true;
        }

        public void UpdateFilter(string filterValue)
        {
            VerifyView();
            
            if (!IsInitialized) return;
            
            var data = _filter.Filter(filterValue,_world);
            if (data.type != ResultType.Success)
                return;
            
            UpdateEntitiesView(data);
        }

        public bool VerifyView()
        {
            if (Application.isPlaying && 
                _world != null)
                return true;
            
            ReleaseEntityViews();
            ResetStatus();
            IsInitialized = false;
            return false;
        }

        public void ResetStatus()
        {
            _cachedFilter = new EcsFilterData();
        }
        
        public void UpdateEntitiesView(EcsFilterData data)
        {
            ReleaseEntityViews();

            _uniqueEntities.Clear();
            _uniqueEntities.UnionWith(data.entities);
            
            foreach (var dataEntity in _uniqueEntities)
            {
                var view = _viewBuilder.Create(dataEntity,_world);
                entities.Add(view);
            }
        }

        public void ReleaseEntityViews()
        {
            foreach (var view in entities)
            {
                view.Release();
                view.Despawn();
            }

            entities.Clear();
        }

        public Color GetStatusColor()
        {
            var resultType = _cachedFilter.type;

            var resultColor = resultType switch
            {
                ResultType.Error => Color.red,
                ResultType.None => Color.yellow,
                ResultType.Success => Color.green,
                _ => Color.magenta,
            };

            return resultColor;
        }

        public Color GetElementColor(int index, Color defaultColor)
        {
            return index % 2 == 0 ? _oddColor : defaultColor;
        }
    }
}