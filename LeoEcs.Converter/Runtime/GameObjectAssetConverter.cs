namespace UniGame.LeoEcsLite.LeoEcs.Converter.Runtime
{
    using System;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Converter.Runtime.Abstract;
    using UnityEngine;

#if TRI_INSPECTOR
    using TriInspector;
#endif
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [CreateAssetMenu(menuName = "UniGame/LeoEcs/Converter/GameObject Converter",fileName = "GameObject Converter")]
    public class GameObjectAssetConverter : ScriptableObject,IEcsComponentConverter
    {
        [HideLabel]
        [InlineProperty]
        public GameObjectConverter converter = new GameObjectConverter();

        public string Name => GetType().Name;
        
        public bool IsEnabled => converter.IsEnabled;
        
        public void Apply(EcsWorld world, int entity)
        {
            converter.Apply(world,entity);
        }

        public void Apply(GameObject target, EcsWorld world, int entity)
        {
            converter.Apply(target,world,entity);
        }

        public bool IsMatch(string searchString)
        {
            if (converter.IsMatch(searchString)) return true;
            if (name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
    }
}