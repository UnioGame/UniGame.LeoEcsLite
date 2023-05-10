namespace UniGame.LeoEcs.Debug.Editor
{
    using System;
    using System.Collections.Generic;
    using Leopotam.EcsLite;
    using Runtime.ObjectPool;
    using Sirenix.OdinInspector;
    using UniModules.UniCore.Runtime.Utils;
    using UnityEngine;

    [Serializable]
    public class EditorEntityViewBuilder
    {
        [SerializeReference]
        [InlineProperty]
        public List<IEntityEditorViewBuilder> viewBuilders = new List<IEntityEditorViewBuilder>()
        {
            new ComponentsEntityBuilder(),
            new GameObjectEntityBuilder(),
        };

        public void Initialize(EcsWorld world)
        {
            foreach (var viewBuilder in viewBuilders)
                viewBuilder.Initialize(world);
        }

        public EntityEditorView Create(int entity,EcsWorld world)
        {
            var view = ClassPool.Spawn<EntityEditorView>();
            view.id = entity;
            view.packedEntity = world.PackEntity(entity);
            view.name = entity.ToStringFromCache();
            
            var packed = world.PackEntity(entity);
            if(packed.Unpack(world, out _) == false) return view;
            
            foreach (var builder in viewBuilders)
                builder.Execute(view);

            return view;
        }
    }
}