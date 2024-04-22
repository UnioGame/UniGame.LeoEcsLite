namespace Game.Ecs.GameAi.MoveToTarget.Converters
{
	using System;
	using System.Threading;
	using Leopotam.EcsLite;
	using UniCore.Runtime.ProfilerTools;
	using UniGame.LeoEcs.Converter.Runtime;
	using UniGame.LeoEcs.Shared.Components;
	using UniGame.LeoEcs.Shared.Extensions;
	using UnityEngine;

	/// <summary>
	/// Add Rigidbody Component to entity
	/// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
	[Serializable]
	public class RigidbodyConverter : LeoEcsConverter
	{
		public override void Apply(GameObject target, EcsWorld world, int entity)
		{
			var rigidbody = target.GetComponent<Rigidbody>();
#if UNITY_EDITOR
			if (!rigidbody)
			{
				GameLog.Log($"{nameof(RigidbodyConverter)} WRONG TARGET",Color.red);
				return;
			}
#endif
			ref var rigidbodyComponent = ref world.GetOrAddComponent<RigidbodyComponent>(entity);
			rigidbodyComponent.Value = rigidbody;
		}
	}
}