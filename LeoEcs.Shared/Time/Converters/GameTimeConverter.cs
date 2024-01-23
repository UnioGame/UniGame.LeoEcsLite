namespace Game.Ecs.Time.Converters
{
    using System;
    using System.Threading;
    using Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Converter.Runtime;
    using UnityEngine;

    [Serializable]
    public class GameTimeConverter : LeoEcsConverter
    {
        public override void Apply(GameObject target, EcsWorld world, int entity)
        {
            var gameTimePool = world.GetPool<EntityGameTimeComponent>();
            gameTimePool.Add(entity);
        }
    }
}