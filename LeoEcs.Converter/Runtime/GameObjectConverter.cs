namespace UniGame.LeoEcs.Converter.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Leopotam.EcsLite;
    using Sirenix.OdinInspector;
    using Abstract;
    using UniGame.LeoEcs.Shared.Components;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;
    
#if UNITY_EDITOR
    using UniModules.Editor;
#endif
    
    [Serializable]
    public class GameObjectConverter 
        : IComponentConverter, ILeoEcsMonoComponentConverter
    {
        [InlineButton(nameof(OpenScript),SdfIconType.Folder2Open)]
        [GUIColor("GetButtonColor")]
        public bool enabled = true;
 
        public virtual string Name => GetType().Name;
        
        public bool IsEnabled => enabled;
        
        public void Apply(EcsWorld world, int entity, CancellationToken cancellationToken = default)
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

            Apply(gameObjectComponent.Value, world, entity, cancellationToken);
        }

        public void Apply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            OnApply(target, world, entity, cancellationToken);
        }

        protected virtual void OnApply(GameObject target, EcsWorld world, int entity, CancellationToken cancellationToken = default)
        {
            
        }

        public bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return true;
            if (GetType().Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            
            return false;
        }

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