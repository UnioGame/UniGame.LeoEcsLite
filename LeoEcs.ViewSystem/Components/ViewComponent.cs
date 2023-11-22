using System;
using UniGame.ViewSystem.Runtime;

namespace UniGame.LeoEcs.ViewSystem.Components
{
    [Serializable]
    public struct ViewComponent
    {
        public IView View;
        public Type Type;
    }
}