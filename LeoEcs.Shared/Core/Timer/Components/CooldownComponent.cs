namespace UniGame.LeoEcs.Timer.Components
{
    using Leopotam.EcsLite;

    public struct CooldownComponent : IEcsAutoReset<CooldownComponent>
    {
        public float Value;
        public bool UnscaleTime;
        
        public void AutoReset(ref CooldownComponent c)
        {
            c.UnscaleTime = false;
            c.Value = 0;
        }
    }
}