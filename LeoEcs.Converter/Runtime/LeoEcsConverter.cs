﻿namespace UniGame.LeoEcs.Converter.Runtime
{
    using System;
    using System.Threading;
    using Abstract;
    using Leopotam.EcsLite;
    using Shared.Components;
    using Shared.Extensions;
    using UnityEngine;

#if TRI_INSPECTOR
    using TriInspector;
#endif
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
#if UNITY_EDITOR
    using UniModules.Editor;
#endif
    
    [Serializable]
    public abstract class LeoEcsConverter : IEcsComponentConverter
    {
        [SerializeField]
#if ODIN_INSPECTOR
        [InlineButton(nameof(OpenScript),SdfIconType.Folder2Open)]
#endif
#if TRI_INSPECTOR || ODIN_INSPECTOR
        [GUIColor("GetButtonColor")]
#endif
        private bool _isEnabled = true;

        public virtual bool IsEnabled => _isEnabled;

        public string Name => GetType().Name;

        public bool IsRuntime => Application.isPlaying;
        
        public abstract void Apply(GameObject target, EcsWorld world, int entity);
        
        public void Apply(EcsWorld world, int entity)
        {
            if (!world.HasComponent<GameObjectComponent>(entity)) return;

            ref var gameObjectComponent = ref world.GetComponent<GameObjectComponent>(entity);

            if (gameObjectComponent.Value == null) return;
            
            Apply(gameObjectComponent.Value,world,entity);
        }
        
        public virtual bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return true;

            if (GetType().Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

#if TRI_INSPECTOR
        [Button]
#endif
        public void OpenScript()
        {
#if UNITY_EDITOR
            GetType().OpenScript();
#endif
        }
        
        private Color GetButtonColor()
        {
#if UNITY_EDITOR
            return _isEnabled ? 
                new Color(0.2f, 1f, 0.2f) : 
                new Color(1, 0.6f, 0.4f);
#endif
            return Color.green;
        }

    }
}