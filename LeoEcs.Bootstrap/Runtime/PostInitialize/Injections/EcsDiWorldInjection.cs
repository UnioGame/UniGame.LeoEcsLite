namespace UniGame.LeoEcs.Bootstrap.Runtime.PostInitialize
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Leopotam.EcsLite;

    [Serializable]
    public class EcsDiWorldInjection : IEcsDiInjection
    {
        private Type _worldType = typeof(EcsWorld);
        private Type _ecsSystems = typeof(IEcsSystems);
        
        public void ApplyInjection(
            IEcsSystems ecsSystems, 
            FieldInfo field, 
            object target, 
            IReadOnlyList<IEcsDiInjection> injections)
        {
            var fieldType = field.FieldType;
            var world = ecsSystems.GetWorld();
            
            if (_worldType == fieldType)
            {
                var value = field.GetValue(target);
                if(value==null) return;
                field.SetValue(target,world);
                return;
            }
            
            if (_ecsSystems == fieldType)
                field.SetValue(target,ecsSystems);
        }
    }
}