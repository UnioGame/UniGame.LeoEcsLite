namespace Game.Ecs.Core
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;
    using UnityEngine;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public static class EntityHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistanceCheckValue IsClosest(float3 sourcePosition, float3 destinationPosition, ref EntityBounds destinationBounds, float minDistance)
        {
            var distance = GetDistance(sourcePosition, destinationPosition, ref destinationBounds);
            var isClosest = distance <= minDistance;
            return new DistanceCheckValue(distance,isClosest);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistanceCheckValue IsSqrClosest(ref float3 sourcePosition,ref  float3 destinationPosition, ref EntityBounds destinationBounds, float minDistance)
        {
            var sqrDistance = GetSqrDistance(ref sourcePosition,ref  destinationPosition, ref destinationBounds);
            var isClosest =  sqrDistance <= minDistance;
            return new DistanceCheckValue(sqrDistance,isClosest);
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetDistance(float3 sourcePosition, float3 destinationPosition, ref EntityBounds destinationBounds)
        {
            var direction = math.normalize(sourcePosition - destinationPosition);
            var closestPoint = destinationPosition + direction * destinationBounds.Radius;
            return math.distance(closestPoint, sourcePosition);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetSqrDistance(ref float3 sourcePosition,ref  float3 destinationPosition, ref EntityBounds destinationBounds)
        {
            var direction = math.normalize(sourcePosition - destinationPosition);
            var closestPoint = destinationPosition + direction * destinationBounds.Radius;
            var sqrDistance = math.distancesq(closestPoint, sourcePosition);
            return sqrDistance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetPoint(float3 sourcePosition, float3 destinationPosition, ref EntityBounds destinationBounds)
        {
            var direction = math.normalize(sourcePosition - destinationPosition);
            var bounds = destinationBounds.Radius;
            var closestPoint = destinationPosition + direction * bounds;
            return closestPoint;
        }
        
    }
    
    [Serializable]
    public readonly struct DistanceCheckValue
    {
        public readonly float distance;
        public readonly bool isClosest;

        public DistanceCheckValue(float distance,bool isClosest)
        {
            this.distance = distance;
            this.isClosest = isClosest;
        }
    }
}