using UnityEditor;

namespace UniGame.LeoEcs.Debug.Editor
{
    using Converter.Runtime;
    using Leopotam.EcsLite;
    using Runtime.ObjectPool;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class EntitiesBrowserWindow : OdinEditorWindow
    {
        #region statics data

        private static Color buttonColor = new Color(0.2f, 1, 0.6f);

        [MenuItem("UniGame/LeoEcs/Entities Browser")]
        [MenuItem("Game/Editors/Entities Browser")]
        public static EntitiesBrowserWindow OpenWindow()
        {
            var window = Create();
            window.Show();
            return window;
        }
    
        public static EntitiesBrowserWindow OpenPopupWindow()
        {
            var window = Create();
            window.ShowPopup();
            return window;
        }

        public static EntitiesBrowserWindow Create()
        {
            var window = GetWindow<EntitiesBrowserWindow>();
            window.titleContent.text = "Entities Browser";
            return window;
        }

        #endregion

        #region inspector
        
        [InlineButton(nameof(UpdateFilter),
            nameof(search),
            Icon = SdfIconType.Search)]
        [HideLabel]
        [EnableIf(nameof(HasEcsWorld))]
        public string search;

        [Space(8)]
        [HorizontalGroup()]
        [LabelWidth(60)]
        [ReadOnly]
        [LabelText("entities :")]
        public int totalEntities;
        
        // [HideInInspector]
        // [InlineButton(nameof(Fill),nameof(Fill),Icon = SdfIconType.TerminalFill)]
        // public int count = 10;

        [HideLabel]
        [BoxGroup("entities")]
        public EntityGridEditorView gridEditorView;

        [HideInInspector]
        [InlineProperty]
        [HideLabel]
        [EnableIf(nameof(HasEcsWorld))]
        public EntitiesEditorView view;

        #endregion

        public bool HasEcsWorld => World != null;

        public EcsWorld World => LeoEcsConvertersData.World;
        
        [PropertyOrder(-1)]
        [ResponsiveButtonGroup()]
        [GUIColor(nameof(buttonColor))]
        [Button(ButtonSizes.Large,Icon = SdfIconType.ArrowClockwise)]
        public void Refresh()
        {
            view = new EntitiesEditorView();
            view.Initialize(LeoEcsConvertersData.World);
            
            Clear();
            
            if(HasEcsWorld && World.IsAlive()) UpdateFilter();
        }

        public void UpdateFilter()
        {
            if(!EntitiesEditorView.IsInitialized)
                view.Initialize(World);
            
            gridEditorView.items.Clear();
            
            view.UpdateFilter(search);

            gridEditorView.items.AddRange(view.entities);
            
            totalEntities = World.GetEntitiesCount();
        }


        private void Clear()
        {
            gridEditorView.items.Clear();
        }

        protected override void Initialize()
        {
            base.Initialize();

            gridEditorView ??= new EntityGridEditorView();
            
            Refresh();
        }

        private string testName = "testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest";
        public void Fill(int amount)
        {
            var items = gridEditorView.items;
            items.Clear();
            
            for (int j = 0; j < amount; j++)
            {
                var id = j;
                var item = ClassPool.Spawn<EntityIdEditorView>();
                item.id = id;
                item.name = testName.Substring(0,Random.Range(1,testName.Length));
                items.Add(item);
            }
        }
    }
}

