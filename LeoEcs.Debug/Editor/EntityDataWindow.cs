namespace UniGame.LeoEcs.Debug.Editor
{
    using System.Collections;
    using System.Collections.Generic;
    using Converter.Runtime;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEngine;

    public class EntityDataWindow : OdinEditorWindow
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
        
        [OnValueChanged(nameof(UpdateView))]
        [InlineButton(nameof(UpdateView),SdfIconType.Arrow90degLeft,"Refresh")]
        public int entityId = -1;
        
        [OnValueChanged(nameof(SetAutoUpdate))]
        public bool autoUpdate = false;

        [TitleGroup("Entity View")]
        [HideLabel]
        [InlineProperty]
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StopAutoRefresh();
        }
        
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