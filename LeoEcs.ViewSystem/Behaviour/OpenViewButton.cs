namespace UniGame.LeoEcs.ViewSystem.Behavriour
{
    using Converter.Runtime;
    using Converter.Runtime.Abstract;
    using Core.Runtime;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;
    using Extensions;
    using UiSystem.Runtime.Settings;
    using UniGame.Rx.Runtime.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Extensions;
    using UniModules.UniGame.UiSystem.Runtime;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(LeoEcsMonoConverter))]
    public class OpenViewButton : MonoBehaviour, IEcsComponentConverter, ILifeTimeContext
    {
        #region inspector

        public bool isEnabled = true;
        
        public Button trigger;
        
        /// <summary>
        /// target view type
        /// </summary>
        public ViewId view;
        
        /// <summary>
        /// target layout
        /// </summary>
        public ViewType layoutType = ViewType.Window;

        #endregion

        private ILifeTime _lifeTime;

        public ILifeTime LifeTime => _lifeTime;

        public bool IsEnabled => isEnabled;

        public string Name => GetType().Name;

        public void Apply(EcsWorld world, int entity)
        {
            _lifeTime = this.GetAssetLifeTime();
            trigger ??= GetComponent<Button>();
            
            this.Bind(trigger, x => world
                .MakeViewRequest(view, layoutType));
        }
        
        [OnInspectorInit]
        private void OnInspectorInitialize()
        {
            if (trigger == null)
                trigger = GetComponent<Button>();
        }

        
        public bool IsMatch(string searchString)
        {
            if(string.IsNullOrEmpty(searchString)) return true;
         
            if(Name.Contains(searchString, System.StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}
