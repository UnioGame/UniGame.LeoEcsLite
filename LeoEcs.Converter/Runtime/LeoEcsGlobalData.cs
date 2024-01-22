﻿namespace UniGame.LeoEcs.Converter.Runtime
{
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

        public static async UniTask<EcsWorld> WaitAliveWorld()
        {
            if (World != null && World.IsAlive()) return World;

            await UniTask.WaitWhile(() => World == null || !World.IsAlive())
                .AttachExternalCancellation(LifeTime.Token);

            return World;
        }
        
        public static async UniTask<T> GetValue<T>()
        {
            await WaitAliveWorld()
                .AttachExternalCancellation(LifeTime.Token);
            
            return World.GetGlobal<T>();
        }
    }
}