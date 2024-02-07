namespace UniGame.LeoEcs.Converter.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using Leopotam.EcsLite;
    using Shared.Extensions;
    using UniModules.UniCore.Runtime.DataFlow;

    public static class LeoEcsGlobalData
    {
        public static EcsWorld World;
        public static LifeTimeDefinition LifeTime;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            World = null;
            LifeTime?.Terminate();
            LifeTime = new LifeTimeDefinition();
        }

        public static bool HasFlag<T>(T flag)
            where T : Enum
        {
            if (World != null && World.IsAlive()) return false;
            return World.HasFlag<T>(flag);
        }

        public static async UniTask<EcsWorld> WaitAliveWorld()
        {
            if (World != null && World.IsAlive()) return World;

            await UniTask.WaitWhile(() => World == null || !World.IsAlive())
                .AttachExternalCancellation(LifeTime.Token);

            return World;
        }
        
        public static T GetGlobal<T>()
        {
            if (World == null || World.IsAlive() == false) return default;
            return World.GetGlobal<T>();
        }
        
        public static async UniTask<T> GetValueAsync<T>()
        {
            await WaitAliveWorld()
                .AttachExternalCancellation(LifeTime.Token);
            
            return World.GetGlobal<T>();
        }
    }
}