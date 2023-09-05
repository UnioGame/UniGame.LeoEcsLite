namespace Game.Ecs.Core.Components
{
    using Leopotam.EcsLite;
    using UnityEngine;

    /// <summary>
    /// Угол вращения при перемещении.
    /// </summary>
    public struct RotationComponent : IEcsAutoReset<RotationComponent>
    {
        public Quaternion Value;
        
        public void AutoReset(ref RotationComponent c)
        {
            c.Value = Quaternion.identity;
        }
    }
}