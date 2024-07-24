namespace UniGame.LeoEcs.Bootstrap.Runtime.PostInitialize
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.Di;
    using Shared.Extensions;

    [Serializable]
    public class EcsDiFilterInjection : IEcsDiInjection
    {
        private Type _worldType = typeof(EcsWorld);
        private Type _ecsSystems = typeof(IEcsSystems);
        private Type _fileterType = typeof(IEcsDataInject);
        
        public void ApplyInjection(
            IEcsSystems ecsSystems, 
            FieldInfo field, 
            object target, 
            IReadOnlyList<IEcsDiInjection> injections)
        {
            var fieldType = field.FieldType;
            var world = ecsSystems.GetWorld();
            
            if(!_fileterType.IsAssignableFrom(fieldType)) return;

            var filter = field.GetValue(target) as IEcsDataInject;
            if (filter == null) return;
            
            filter.Fill(ecsSystems);
            field.SetValue(target,filter);
        }
    }
}