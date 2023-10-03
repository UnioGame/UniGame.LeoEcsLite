﻿namespace UniGame.LeoEcs.ViewSystem.Components
{
    using System;
    using System.Collections.Generic;
    using Leopotam.EcsLite;
    using UniModules.UniCore.Runtime.Utils;
    using UniModules.UniGame.UiSystem.Runtime;
    using UnityEngine.Serialization;

    [Serializable]
    public struct CreateLayoutViewRequest : IEcsAutoReset<CreateLayoutViewRequest>
    {
        public string View;
        public string LayoutType;

        public void AutoReset(ref CreateLayoutViewRequest c)
        {
            c.View = string.Empty;
            c.LayoutType = ViewType.Window.ToStringFromCache();
        }
    }
}