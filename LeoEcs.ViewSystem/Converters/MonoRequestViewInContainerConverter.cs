namespace UniGame.LeoEcs.ViewSystem.Converters
{
    using System;
    using System.Threading;
    using Components;
    using Leopotam.EcsLite;
    using Shared.Extensions;
    using Sirenix.OdinInspector;
    using UiSystem.Runtime.Settings;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Converter.Runtime.Converters;
    using UnityEngine;

    public sealed class MonoRequestViewInContainerConverter : MonoLeoEcsConverter<RequestViewInContainerConverter>
    {

    }

    [Serializable]
    public class RequestViewInContainerConverter : LeoEcsConverter
    {
        [TitleGroup("View Data")]
        public ViewId view;
        
        [TitleGroup("View Data")]
        public bool useBusyContainer = false;
        
        [TitleGroup("View Data")]
        public bool ownView = false;
        
        /// <summary>
        /// Optional Data
        /// </summary>
        [TitleGroup("View Data")]
        [Optional]
        public string Tag = string.Empty;
        
        /// <summary>
        /// Optional Data
        /// </summary>
        [TitleGroup("View Data")]
        [Optional]
        public string ViewName = string.Empty;
        
        /// <summary>
        /// Optional Data
        /// </summary>
        [TitleGroup("View Data")]
        [Optional]
        public bool StayWorld;
        
        public sealed override void Apply(GameObject target, EcsWorld world, int entity)
        {
            
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(view))
            {
                Debug.LogError($"View Is is Empty for Create in Container {target.name}",target);
                return;
            }
#endif
            
            var requestEntity = world.NewEntity();
            ref var request = ref world.AddComponent<CreateViewInContainerRequest>(requestEntity);

            request.View = view;
            request.UseBusyContainer = useBusyContainer;
            request.Tag = Tag;
            request.ViewName = ViewName;
            request.StayWorld = StayWorld;

            if (ownView) request.Owner = world.PackEntity(entity);
        }       
    }
}