using System;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game.Ecs.Core.Components
{
    [Serializable]
    public struct CharacterLinkComponent
    {
        public EcsPackedEntity CharacterEntity;
    }
}
