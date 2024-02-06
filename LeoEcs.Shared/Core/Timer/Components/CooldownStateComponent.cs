namespace UniGame.LeoEcs.Timer.Components
{
    using Leopotam.EcsLite;

    /// <summary>
    /// Состояние отката.
    /// </summary>
    public struct CooldownStateComponent : IEcsAutoReset<CooldownStateComponent>
    {
        /// <summary>
        /// time of last update.
        /// </summary>
        public float LastTime;

        public void AutoReset(ref CooldownStateComponent c)
        {
            c.LastTime = 0;
        }
    }
}