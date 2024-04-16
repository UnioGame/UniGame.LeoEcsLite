﻿namespace UniGame.LeoEcs.Debug.Editor
{
    using System;
    using System.Buffers;
    using Converter.Runtime;
    using Core.Runtime.ObjectPool;
    using Leopotam.EcsLite;
    using Runtime.ObjectPool.Extensions;
    using UniModules.Editor;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    
    [Serializable]
    public class ComponentEditorView : IPoolable,ISearchFilterable
    {
        private const string BoxGroupLabel = "@Label";
        private const string ComponentKey = "Component";
        private const string ComponentGroupValue = "Component/Value";
        private const string ComponentGroupActions = "Component/Actions";
        
        #region inspector
        
        [HideInInspector]
        public int entity;
        
        [HorizontalGroup(ComponentKey, 0.75f)]
        [BoxGroup(ComponentGroupValue,LabelText = BoxGroupLabel)]
        [InlineProperty]
        [HideLabel]
        [OnInspectorGUI(Prepend = nameof(DrawComponentPrepend))]
        [HideReferenceObjectPicker]
        public object value;

        #endregion
        
        public string Label => value == null ? ComponentKey : value.GetType().GetFormattedName();

        public EcsWorld World => LeoEcsGlobalData.World;

        public bool IsAlive => World != null && World.IsAlive();
        
        [BoxGroup(ComponentGroupActions)]
        [Button(nameof(OpenScript),ButtonSizes.Small, Icon = SdfIconType.Folder2Open)]
        public void OpenScript()
        {
            if (value == null) return;
            value.GetType().OpenScript();
        }
        
        [BoxGroup(ComponentGroupActions)]
        [Button(nameof(ApplyChanges),ButtonSizes.Small, Icon = SdfIconType.Magic)]
        public void ApplyChanges()
        {
            if (value == null || !IsAlive) return;

            var world = World;
            var count = world.GetPoolsCount();
            var pools = ArrayPool<IEcsPool>.Shared.Rent(count);
            world.GetAllPools(ref pools);
            foreach (var pool in pools)
            {
                if(pool == null)continue;
                if(pool.GetComponentType()!= value.GetType()) continue;
                pool.Del(entity);
                pool.AddRaw(entity,value);
                break;
            }
            pools.Despawn();
        }
        
        [BoxGroup(ComponentGroupActions)]
        [Button(nameof(Remove),ButtonSizes.Small, Icon = SdfIconType.Eraser)]
        public void Remove()
        {
            if (value == null || !IsAlive) return;

            var world = World;
            var count = world.GetPoolsCount();
            var pools = ArrayPool<IEcsPool>.Shared.Rent(count);
            world.GetAllPools(ref pools);
            foreach (var pool in pools)
            {
                if(pool == null)continue;
                if(pool.GetComponentType()!= value.GetType()) continue;
                pool.Del(entity);
            }
            pools.Despawn();
        }
        
        public void Release()
        {
            value = null;
        }


        private void DrawComponentPrepend()
        {
            
        }

        public bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return true;
            if (value == null) return false;
            return value.GetType().Name.Contains(searchString, StringComparison.OrdinalIgnoreCase);
        }
    }
}