namespace Game.Ecs.TargetSelection
{
    using System;
    using Leopotam.EcsLite;

    [Serializable]
    public readonly struct EntityPackedFloat
    {
        public readonly EcsPackedEntity entity;
        public readonly float value;

        public EntityPackedFloat(EcsPackedEntity entity,float distance)
        {
            this.entity = entity;
            this.value = distance;
        }
    }
}