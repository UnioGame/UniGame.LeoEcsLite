namespace Game.Ecs.TargetSelection
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Unity.Burst;

    [BurstCompile]
    public struct EntityFloatComparer : IComparer<EntityFloat>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(EntityFloat x, EntityFloat y)
        {
            if (x.entity == 0) return 1;
            if (y.entity == 0) return -1;
            if (x.value - y.value < 0) return -1;
            return 1;
        }
    }
}