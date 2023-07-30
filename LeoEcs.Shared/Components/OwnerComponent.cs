namespace Game.Ecs.Core.Components
{
    using System;
    using Leopotam.EcsLite;

    /// <summary>
    /// owner entity
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct OwnerComponent
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.OnInspectorGUI]
#endif
        public EcsPackedEntity Entity;
    }
}