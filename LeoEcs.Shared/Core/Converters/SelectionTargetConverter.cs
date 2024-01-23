using System;
using Game.Ecs.Core.Components;
using Leopotam.EcsLite;
using UniGame.LeoEcs.Converter.Runtime;
using UniGame.LeoEcs.Shared.Extensions;

namespace Game.Ecs.Core.Converters
{
    [Serializable]
    public class SelectionTargetConverter : EcsComponentConverter
    {
        public override void Apply(EcsWorld world, int entity)
        {
            world.GetOrAddComponent<SelectionTargetComponent>(entity);
        }
    }
}
