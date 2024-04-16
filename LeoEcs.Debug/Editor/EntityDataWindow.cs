namespace UniGame.LeoEcs.Debug.Editor
{
    using System.Collections;
    using System.Collections.Generic;
    using Converter.Runtime;
    using Unity.EditorCoroutines.Editor;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
#endif
    
    public class EntityDataWindow 
#if ODIN_INSPECTOR
        : OdinEditorWindow
#else
        : EditorWindow
#endif
    {
        #region statics data

        private static Color buttonColor = new Color(0.2f, 1, 0.6f);

        [MenuItem("UniGame/LeoEcs/Entity Data Window")]
        [MenuItem("Game/Editors/Entity Data Window")]
        public static EntityDataWindow OpenWindow()
        {
            var window = Create(-1);
            window.Show();
            return window;
        }
    
        public static EntityDataWindow OpenPopupWindow(int entityId)
        {
            var window = Create(entityId);
            window.ShowPopup();
            return window;
        }

        public static EntityDataWindow Create(int entityId)
        {
            var window = GetWindow<EntityDataWindow>();
            window.titleContent.text = "Entity Data Window";
            window.entityId = entityId;
            window.UpdateView();
            return window;
        }

        #endregion

        #region inspector
        
#if ODIN_INSPECTOR
        [OnValueChanged(nameof(UpdateView))]
        [InlineButton(nameof(UpdateView),SdfIconType.Arrow90degLeft,"Refresh")]
#endif
        public int entityId = -1;
        
#if ODIN_INSPECTOR
        [OnValueChanged(nameof(SetAutoUpdate))]
#endif
        public bool autoUpdate = false;

#if ODIN_INSPECTOR
        [TitleGroup("Entity View")]
        [HideLabel]
        [InlineProperty]
#endif
        public EntityEditorView entityView;

        #endregion
        
        private EditorEntityViewBuilder _viewBuilder = new EditorEntityViewBuilder();
        private EditorCoroutine _coroutine;
        private float _updateDelay = 1f;

        public void UpdateView()
        {
            if (entityId < 0) return;
            var world = LeoEcsGlobalData.World;
            if (world == null || world.IsAlive() == false) return;
            
            _viewBuilder.Initialize(world);
            entityView = _viewBuilder.Create(entityId, world);
        }

        public void SetAutoUpdate(bool enabled)
        {
            StopAutoRefresh();
            
            if (!enabled) return;
            
            EditorCoroutineUtility.StartCoroutine(AutoRefresh(), this);
        }

#if ODIN_INSPECTOR
        protected override void OnDestroy()
        {
            base.OnDestroy();
            StopAutoRefresh();
        }
#endif
        
        private void StopAutoRefresh()
        {
            if (_coroutine == null) return;
            EditorCoroutineUtility.StopCoroutine(_coroutine);
            _coroutine = null;
        }
        
        private IEnumerator AutoRefresh()
        {
            var waitForOneSecond = new EditorWaitForSeconds(_updateDelay);

            while (_coroutine!=null)
            {
                yield return waitForOneSecond;

                UpdateView();
            }
        }

    }
}