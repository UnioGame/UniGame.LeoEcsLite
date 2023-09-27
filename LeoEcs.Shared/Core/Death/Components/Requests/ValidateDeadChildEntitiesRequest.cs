namespace Game.Ecs.Core.Death.Components
{
    using Leopotam.EcsLite;

    /// <summary>
    /// validate all dead child entities in single frame
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public struct ValidateDeadChildEntitiesRequest : IEcsAutoReset<ValidateDeadChildEntitiesRequest>
    {
        public bool ForceDestroy;
        
        public void AutoReset(ref ValidateDeadChildEntitiesRequest c)
        {
            c.ForceDestroy = false;
        }
    }
}