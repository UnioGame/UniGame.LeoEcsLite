namespace UniGame.LeoEcs.Bootstrap.Runtime.PostInitialize
{
    using System;
    using System.Reflection;
    using Abstract;
    using Attributes;
    using Leopotam.EcsLite;
    using UniModules.UniCore.Runtime.ReflectionUtils;

    [Serializable]
    public class EcsDiPostInitialize : IEcsPostInitializeAction
    {
        private const string PoolMethodName = "GetPool";
        
        private MethodInfo _poolMethod;
        private Type _diAttributeType = typeof(ECSDIAttribute);
        private Type _poolType = typeof(EcsPool<>);
        
        public void Apply(IEcsSystems ecsSystems,IEcsSystem system)
        {
            var world = ecsSystems.GetWorld();
            if (world == null) return;
            
            _poolMethod ??= world.GetType().GetMethod(PoolMethodName);
            
            if(_poolMethod == null) return;
            
            var systemType = system.GetType();
            var isDiSystem = systemType.HasAttribute<ECSDIAttribute>();
            var fields = systemType.GetInstanceFields();
            
            foreach (var field in fields)
            {
                ApplyPoolInjection(world,field,system,isDiSystem);
            }
        }

        private void ApplyPoolInjection(EcsWorld world,FieldInfo field,object target,bool isDiSystem)
        {
            var fieldType = field.FieldType;

            if (!fieldType.IsGenericType) return;
            
            var isDiTarget = isDiSystem || Attribute.IsDefined (field, _diAttributeType);
            if(!isDiTarget) return;
                
            var isPoolType = fieldType.GetGenericTypeDefinition() == _poolType;
            if(!isPoolType) return;
                
            var fieldValue = field.GetValue(target);
            if(fieldValue!=null) return;
                
            var elementType = fieldType.GetGenericArguments()[0];
            var poolGenericMethod = _poolMethod.MakeGenericMethod(elementType);
            var result = poolGenericMethod.Invoke(world,null);
            
            field.SetValue(target,result);
        }
    }
    
    
}