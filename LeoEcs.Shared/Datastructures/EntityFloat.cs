namespace Game.Ecs.TargetSelection
{
    using System;

    [Serializable]
    public readonly struct EntityFloat
    {
        public readonly int entity;
        public readonly float value;

        public EntityFloat(int entity,float value)
        {
            this.entity = entity;
            this.value = value;
        }
    }
}