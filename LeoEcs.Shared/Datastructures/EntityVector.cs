namespace Game.Ecs.TargetSelection
{
    using System;
    using Unity.Mathematics;

    [Serializable]
    public readonly struct EntityVector
    {
        public readonly int entity;
        public readonly float3 point;

        public EntityVector(int entity,float3 distance)
        {
            this.entity = entity;
            this.point = distance;
        }
    }
}