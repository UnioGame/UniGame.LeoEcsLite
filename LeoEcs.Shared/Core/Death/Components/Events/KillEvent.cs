namespace Game.Ecs.Core.Death.Components
{
    using Leopotam.EcsLite;

    /// <summary>
    /// separate entity event with source and dest targets
    /// </summary>
    public struct KillEvent
    {
        public EcsPackedEntity Source;
        public EcsPackedEntity Destination;
    }
}