﻿namespace Game.Ecs.Core.Components
{
	using System;
	using Leopotam.EcsLite;

	/// <summary>
	/// separate entity event with source and dest targets
	/// </summary>
#if ENABLE_IL2CPP
	using Unity.IL2CPP.CompilerServices;
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
	public struct PrepareToDeathEvent
	{
		public EcsPackedEntity Source;
	}
}