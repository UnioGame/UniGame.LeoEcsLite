namespace UniGame.LeoEcs.ViewSystem.Behavriour
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Converter.Runtime;
    using Converter.Runtime.Abstract;
    using Core.Runtime;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;
    using UniGame.Core.Runtime.SerializableType;
    using UniGame.Core.Runtime.SerializableType.Attributes;
    using Converters;
    using Extensions;
    using UISystem.Runtime.Utils;
    using UniGame.Rx.Runtime.Extensions;
    using UniGame.ViewSystem.Runtime;
    using UniModules.UniGame.Core.Runtime.DataFlow.Extensions;
    using UniModules.UniGame.UiSystem.Runtime;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(LeoEcsMonoConverter))]
    public class OpenViewButton : MonoBehaviour, ILeoEcsComponentConverter, ILifeTimeContext
    {
        #region inspector

        public Button trigger;
        
        /// <summary>
        /// target view type
        /// </summary>
#if ODIN_INSPECTOR
        [ValueDropdown(nameof(GetViewTypes))]
#endif
        public string view;
        
        /// <summary>
        /// target layout
        /// </summary>
        public ViewType layoutType = ViewType.Window;

        #endregion

        private ILifeTime _lifeTime;

        public ILifeTime LifeTime => _lifeTime;

        public void Apply(GameObject target, EcsWorld world, 
            int entity, CancellationToken cancellationToken = default)
        {
            this.Bind(trigger, x => world
                .MakeViewRequest(view, layoutType));
        }
        
        [OnInspectorInit]
        private void OnInspectorInitialize()
        {
            if (trigger == null)
                trigger = GetComponent<Button>();
        }

        private void Awake()
        {
            _lifeTime = this.GetAssetLifeTime();
            trigger ??= GetComponent<Button>();
        }

        private IEnumerable<string> GetViewTypes()
        {
            return ViewSystemTool.GetViewNames();
        }
    }
}
