namespace Game.Ecs.Core.Components
{
    using UnityEngine;

    /// <summary>
    /// Компонент со ссылкой на рутовый transform для эффектов на entity.
    /// </summary>
    public struct EntityAvatarComponent
    {
        public EntityBounds Bounds;
        
        public Transform Head;
        public Transform Body;
        public Transform Feet;
        public Transform Hand;
        
        public Transform Weapon;
    }
}