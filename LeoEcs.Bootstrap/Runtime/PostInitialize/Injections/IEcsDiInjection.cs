namespace UniGame.LeoEcs.Bootstrap.Runtime.PostInitialize
{
    using System.Collections.Generic;
    using System.Reflection;
    using Leopotam.EcsLite;

    public interface IEcsDiInjection
    {
        void ApplyInjection(IEcsSystems ecsSystems, FieldInfo field, object target, IReadOnlyList<IEcsDiInjection> injections);
    }
}