namespace UniGame.LeoEcs.Timer.Components
{
    using Leopotam.EcsLite;

    /// <summary>
    /// Состояние отката.
    /// </summary>
    public struct CooldownStateComponent : IEcsAutoReset<CooldownStateComponent>
    {
        /// <summary>
        /// Время последнего использования умения.
        /// </summary>
        public float LastTime;

        public void AutoReset(ref CooldownStateComponent c)
        {
            c.LastTime = 0;
        }
    }
}