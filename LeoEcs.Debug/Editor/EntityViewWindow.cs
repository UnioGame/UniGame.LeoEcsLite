namespace UniGame.LeoEcs.Debug.Editor
{
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
#endif
    
    using UnityEditor;
    using UnityEngine;

    public class EntityViewWindow
#if ODIN_INSPECTOR
        : OdinEditorWindow
#else
        : EditorWindow
#endif
    {
        #region statics data

        private static Color buttonColor = new Color(0.2f, 1, 0.6f);

        public static EntityViewWindow OpenWindow(EntityEditorView view)
        {
            var window = Create(view);
            window.Show();
            return window;
        }
    
        public static EntityViewWindow OpenPopupWindow(EntityEditorView view)
        {
            var window = Create(view);
            window.ShowPopup();
            return window;
        }

        public static EntityViewWindow Create(EntityEditorView view)
        {
            var window = GetWindow<EntityViewWindow>();
            window.titleContent.text = "Entities Debug View";
            window.entityView = view;
            return window;
        }

        #endregion

        #region inspector

#if ODIN_INSPECTOR
        [HideLabel]
        [InlineProperty]
#endif
        public EntityEditorView entityView;

        #endregion

    }
}