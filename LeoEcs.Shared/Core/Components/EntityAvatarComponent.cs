namespace Game.Ecs.Core.Components
{
    using System;
    using Leopotam.EcsLite;
    using UnityEngine;

    /// <summary>
    /// Компонент со ссылкой на рутовый transform для эффектов на entity.
    /// </summary>
    public struct EntityAvatarComponent : IEcsAutoReset<EntityAvatarComponent>
    {
        public EntityBounds Bounds;
        
        public Transform Head;
        public Transform Body;
        public Transform Feet;
        public Transform Hand;
        public Transform Weapon;
        
        public Transform[] All;
        
        public void AutoReset(ref EntityAvatarComponent c)
        {
            c.Bounds = default;
            c.Head = default;
            c.Body = default;
            c.Feet = default;
            c.Hand = default;
            c.Weapon = default;
            
            c.All = Array.Empty<Transform>();
        }
    }
}