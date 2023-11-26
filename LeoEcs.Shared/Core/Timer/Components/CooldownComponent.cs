namespace UniGame.LeoEcs.Timer.Components
{
    using Leopotam.EcsLite;

    public struct CooldownComponent : IEcsAutoReset<CooldownComponent>
    {
        public float Value;
        
        public void AutoReset(ref CooldownComponent c)
        {
            c.Value = 0;
        }
    }
}