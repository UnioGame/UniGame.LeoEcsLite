namespace Game.Ecs.Core
{
    using System.Runtime.CompilerServices;
    using Code.GameTools.Runtime;
    using UnityEngine;

    public static class EntityHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsClosest(ref Vector3 sourcePosition, ref Vector3 destinationPosition, ref EntityBounds destinationBounds, float minDistance)
        {
            var distance = GetDistance(ref sourcePosition, ref destinationPosition, ref destinationBounds);

            return distance <= minDistance || Mathf.Approximately(distance, minDistance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetDistance(ref Vector3 sourcePosition, ref Vector3 destinationPosition, ref EntityBounds destinationBounds)
        {
            var direction = Vector3.Normalize(sourcePosition - destinationPosition);
            var closestPoint = destinationPosition + direction * destinationBounds.Radius;
            
            return Vector3.Distance(closestPoint, sourcePosition);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetSqrDistance(ref Vector3 sourcePosition, ref Vector3 destinationPosition, ref EntityBounds destinationBounds)
        {
            var direction = Vector3.Normalize(sourcePosition - destinationPosition);
            var closestPoint = destinationPosition + direction * destinationBounds.Radius;
            var sqrDistance = Vector3.SqrMagnitude(closestPoint - sourcePosition);
            return sqrDistance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetPoint(ref Vector3 sourcePosition, ref Vector3 destinationPosition, ref EntityBounds destinationBounds)
        {
            var direction = Vector3.Normalize(sourcePosition - destinationPosition);
            var closestPoint = destinationPosition + direction * destinationBounds.Radius;

            return closestPoint;
        }
    }
}