namespace UniGame.LeoEcs.Converter.Runtime
{
    using System;
    using Leopotam.EcsLite;
    using Abstract;
    using UniGame.LeoEcs.Shared.Components;
    using UniGame.LeoEcs.Shared.Extensions;
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
    public class GameObjectConverter : IEcsComponentConverter
    {
#if ODIN_INSPECTOR
        [InlineButton(nameof(OpenScript),SdfIconType.Folder2Open)]
#endif
#if  ODIN_INSPECTOR || TRI_INSPECTOR
        [GUIColor("GetButtonColor")]
#endif
        public bool enabled = true;
 
        public virtual string Name => GetType().Name;
        
        public bool IsEnabled => enabled;
        
        public void Apply(EcsWorld world, int entity)
        {
            var haveComponent = world.HasComponent<GameObjectComponent>(entity);
            if (!haveComponent)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Converter {GetType().Name} doesn't have {nameof(GameObjectComponent)} on entity {entity}");
#endif
                return;
            }
            
            ref var gameObjectComponent = ref world.GetComponent<GameObjectComponent>(entity);

            Apply(gameObjectComponent.Value, world, entity);
        }

        public void Apply(GameObject target, EcsWorld world, int entity)
        {
            ref var gameObjectComponent = ref world
                .GetOrAddComponent<GameObjectComponent>(entity);
            gameObjectComponent.Value = target;
            
            OnApply(target, world, entity);
        }

        protected virtual void OnApply(GameObject target, EcsWorld world, int entity)
        {
            
        }

        public bool IsMatch(string searchString)
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
            this.GetType().OpenScript();
#endif
        }
        
        private Color GetButtonColor()
        {
#if UNITY_EDITOR
            return enabled ? 
                new Color(0.2f, 1f, 0.2f) : 
                new Color(1, 0.6f, 0.4f);
#endif
            return Color.green;
        }
        
    }
}