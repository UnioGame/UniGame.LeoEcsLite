namespace Game.Ecs.Core.Death.Converters
{
    using System.Threading;
    using Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;

    public sealed class CanDisableMonoConverter : MonoLeoEcsConverter
    {
        [SerializeField] 
        public bool _addDisabledOnAwake = true;
        
        public override void Apply(GameObject target, EcsWorld world, int entity)
        {
            world.AddComponent<CanDisableComponent>(entity);

            if (_addDisabledOnAwake)
                world.AddComponent<DisabledComponent>(entity);
        }
    }
}