// Copyright (c) 2023 Vladimir Popov zor1994@gmail.com https://github.com/ZorPastaman/UtilityAI

using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Profiling;
using Zor.SimpleBlackboard.Core;

namespace Zor.UtilityAI.Core
{
	/// <summary>
	/// Utility AI consideration.
	/// </summary>
	public abstract class Consideration
	{
        private static readonly ProfilerMarker ConsiderationCreateMarker = new($"{nameof(Consideration)}.{nameof(Create)}");

#if ENABLE_PROFILER
        private static readonly ConcurrentDictionary<Type, ProfilerMarker> ConsiderationMarkers = new();
        internal static ProfilerMarker GetConsiderationMarker(Consideration consideration)
        {
            return GetConsiderationMarker(consideration.GetType());
        }

        internal static ProfilerMarker GetConsiderationMarker<T>()
            where T : Consideration
        {
            return GetConsiderationMarker(typeof(T));
        }

        internal static ProfilerMarker GetConsiderationMarker(Type type)
        {
            return ConsiderationMarkers.GetOrAdd(type, static considerationType => new ProfilerMarker(considerationType.FullName));
        }
#endif

		/// <summary>
		/// Used <see cref="Blackboard"/>. Set via <see cref="Brain"/>.
		/// </summary>
		private Blackboard m_blackboard;

		/// <summary>
		/// Consideration's name. It's used for debugging mainly.
		/// </summary>
		public string name { get; set; }

		/// <summary>
		/// Used <see cref="Blackboard"/>. Set via <see cref="Brain"/>.
		/// </summary>
		[NotNull]
		protected Blackboard blackboard
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
			get => m_blackboard;
		}

		/// <summary>
		/// Computes utility of the consideration.
		/// </summary>
		/// <returns>Computed utility.</returns>
		public abstract float ComputeUtility();

		/// <summary>
		/// The method is called once before a first tick of <see cref="Brain"/>.
		/// </summary>
		protected virtual void OnInitialize() {}
		/// <summary>
		/// The method is called when <see cref="Brain"/> is disposed.
		/// </summary>
		protected virtual void OnDispose() {}

		/// <summary>
		/// Initializes a consideration.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Initialize()
		{
#if ENABLE_PROFILER
            using (GetConsiderationMarker(this).Auto())
            {
#endif
                OnInitialize();
#if ENABLE_PROFILER
            }
#endif
		}

		/// <summary>
		/// Disposes a consideration.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Dispose()
		{
#if ENABLE_PROFILER
            using (GetConsiderationMarker(this).Auto())
            {
#endif
                OnDispose();
#if ENABLE_PROFILER
            }
#endif
		}

		/// <summary>
		/// Sets <see cref="Blackboard"/> into a consideration.
		/// </summary>
		/// <param name="blackboardToSet"><see cref="Blackboard"/> to set.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void SetBlackboard([NotNull] Blackboard blackboardToSet)
		{
			m_blackboard = blackboardToSet;
		}

		/// <summary>
		/// Creates a consideration.
		/// </summary>
		/// <typeparam name="TConsideration">Consideration type.</typeparam>
		/// <returns>Created consideration</returns>
		[NotNull]
		public static TConsideration Create<TConsideration>() where TConsideration : Consideration, INotSetupable, new()
		{
            using (ConsiderationCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetConsiderationMarker<TConsideration>().Auto())
                {
#endif
                    return new TConsideration();
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates a consideration.
		/// </summary>
		/// <param name="arg">Argument in a setup method.</param>
		/// <typeparam name="TConsideration">Consideration type.</typeparam>
		/// <typeparam name="TArg">Argument in a setup method type.</typeparam>
		/// <returns>Created consideration.</returns>
		[NotNull]
		public static TConsideration Create<TConsideration, TArg>([CanBeNull] TArg arg) where TConsideration : Consideration, ISetupable<TArg>, new()
		{
            using (ConsiderationCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetConsiderationMarker<TConsideration>().Auto())
                {
#endif
                    var consideration = new TConsideration();
                    consideration.Setup(arg);

                    return consideration;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates a consideration.
		/// </summary>
		/// <param name="arg0">First argument in a setup method.</param>
		/// <param name="arg1">Second argument in a setup method.</param>
		/// <typeparam name="TConsideration">Consideration type.</typeparam>
		/// <typeparam name="TArg0">First argument in a setup method type.</typeparam>
		/// <typeparam name="TArg1">Second argument in a setup method type.</typeparam>
		/// <returns>Created consideration.</returns>
		[NotNull]
		public static TConsideration Create<TConsideration, TArg0, TArg1>([CanBeNull] TArg0 arg0,[CanBeNull]  TArg1 arg1)
			where TConsideration : Consideration, ISetupable<TArg0, TArg1>, new()
		{
            using (ConsiderationCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetConsiderationMarker<TConsideration>().Auto())
                {
#endif
                    var consideration = new TConsideration();
                    consideration.Setup(arg0, arg1);

                    return consideration;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates a consideration.
		/// </summary>
		/// <param name="arg0">First argument in a setup method.</param>
		/// <param name="arg1">Second argument in a setup method.</param>
		/// <param name="arg2">Third argument in a setup method.</param>
		/// <typeparam name="TConsideration">Consideration type.</typeparam>
		/// <typeparam name="TArg0">First argument in a setup method type.</typeparam>
		/// <typeparam name="TArg1">Second argument in a setup method type.</typeparam>
		/// <typeparam name="TArg2">Third argument in a setup method type.</typeparam>
		/// <returns>Created consideration.</returns>
		[NotNull]
		public static TConsideration Create<TConsideration, TArg0, TArg1, TArg2>([CanBeNull] TArg0 arg0, [CanBeNull] TArg1 arg1, [CanBeNull] TArg2 arg2)
			where TConsideration : Consideration, ISetupable<TArg0, TArg1, TArg2>, new()
		{
            using (ConsiderationCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetConsiderationMarker<TConsideration>().Auto())
                {
#endif
                    var consideration = new TConsideration();
                    consideration.Setup(arg0, arg1, arg2);

                    return consideration;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates a consideration.
		/// </summary>
		/// <param name="arg0">First argument in a setup method.</param>
		/// <param name="arg1">Second argument in a setup method.</param>
		/// <param name="arg2">Third argument in a setup method.</param>
		/// <param name="arg3">Fourth argument in a setup method.</param>
		/// <typeparam name="TConsideration">Consideration type.</typeparam>
		/// <typeparam name="TArg0">First argument in a setup method type.</typeparam>
		/// <typeparam name="TArg1">Second argument in a setup method type.</typeparam>
		/// <typeparam name="TArg2">Third argument in a setup method type.</typeparam>
		/// <typeparam name="TArg3">Fourth argument in a setup method type.</typeparam>
		/// <returns>Created consideration.</returns>
		[NotNull]
		public static TConsideration Create<TConsideration, TArg0, TArg1, TArg2, TArg3>([CanBeNull] TArg0 arg0, [CanBeNull] TArg1 arg1, [CanBeNull] TArg2 arg2, [CanBeNull] TArg3 arg3)
			where TConsideration : Consideration, ISetupable<TArg0, TArg1, TArg2, TArg3>, new()
		{
            using (ConsiderationCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetConsiderationMarker<TConsideration>().Auto())
                {
#endif
                    var consideration = new TConsideration();
                    consideration.Setup(arg0, arg1, arg2, arg3);

                    return consideration;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates a consideration.
		/// </summary>
		/// <param name="arg0">First argument in a setup method.</param>
		/// <param name="arg1">Second argument in a setup method.</param>
		/// <param name="arg2">Third argument in a setup method.</param>
		/// <param name="arg3">Fourth argument in a setup method.</param>
		/// <param name="arg4">Fifth argument in a setup method.</param>
		/// <typeparam name="TConsideration">Consideration type.</typeparam>
		/// <typeparam name="TArg0">First argument in a setup method type.</typeparam>
		/// <typeparam name="TArg1">Second argument in a setup method type.</typeparam>
		/// <typeparam name="TArg2">Third argument in a setup method type.</typeparam>
		/// <typeparam name="TArg3">Fourth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg4">Fifth argument in a setup method type.</typeparam>
		/// <returns>Created consideration.</returns>
		[NotNull]
		public static TConsideration Create<TConsideration, TArg0, TArg1, TArg2, TArg3, TArg4>([CanBeNull] TArg0 arg0, [CanBeNull] TArg1 arg1, [CanBeNull] TArg2 arg2, [CanBeNull] TArg3 arg3, [CanBeNull] TArg4 arg4)
			where TConsideration : Consideration, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4>, new()
		{
            using (ConsiderationCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetConsiderationMarker<TConsideration>().Auto())
                {
#endif
                    var consideration = new TConsideration();
                    consideration.Setup(arg0, arg1, arg2, arg3, arg4);

                    return consideration;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates a consideration.
		/// </summary>
		/// <param name="arg0">First argument in a setup method.</param>
		/// <param name="arg1">Second argument in a setup method.</param>
		/// <param name="arg2">Third argument in a setup method.</param>
		/// <param name="arg3">Fourth argument in a setup method.</param>
		/// <param name="arg4">Fifth argument in a setup method.</param>
		/// <param name="arg5">Sixth argument in a setup method.</param>
		/// <typeparam name="TConsideration">Consideration type.</typeparam>
		/// <typeparam name="TArg0">First argument in a setup method type.</typeparam>
		/// <typeparam name="TArg1">Second argument in a setup method type.</typeparam>
		/// <typeparam name="TArg2">Third argument in a setup method type.</typeparam>
		/// <typeparam name="TArg3">Fourth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg4">Fifth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg5">Sixth argument in a setup method type.</typeparam>
		/// <returns>Created consideration.</returns>
		[NotNull]
		public static TConsideration Create<TConsideration, TArg0, TArg1, TArg2, TArg3, TArg4, TArg5>([CanBeNull] TArg0 arg0, [CanBeNull] TArg1 arg1, [CanBeNull] TArg2 arg2, [CanBeNull] TArg3 arg3, [CanBeNull] TArg4 arg4, [CanBeNull] TArg5 arg5)
			where TConsideration : Consideration, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5>, new()
		{
            using (ConsiderationCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetConsiderationMarker<TConsideration>().Auto())
                {
#endif
                    var consideration = new TConsideration();
                    consideration.Setup(arg0, arg1, arg2, arg3, arg4, arg5);

                    return consideration;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates a consideration.
		/// </summary>
		/// <param name="arg0">First argument in a setup method.</param>
		/// <param name="arg1">Second argument in a setup method.</param>
		/// <param name="arg2">Third argument in a setup method.</param>
		/// <param name="arg3">Fourth argument in a setup method.</param>
		/// <param name="arg4">Fifth argument in a setup method.</param>
		/// <param name="arg5">Sixth argument in a setup method.</param>
		/// <param name="arg6">Seventh argument in a setup method.</param>
		/// <typeparam name="TConsideration">Consideration type.</typeparam>
		/// <typeparam name="TArg0">First argument in a setup method type.</typeparam>
		/// <typeparam name="TArg1">Second argument in a setup method type.</typeparam>
		/// <typeparam name="TArg2">Third argument in a setup method type.</typeparam>
		/// <typeparam name="TArg3">Fourth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg4">Fifth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg5">Sixth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg6">Seventh argument in a setup method type.</typeparam>
		/// <returns>Created consideration.</returns>
		[NotNull]
		public static TConsideration Create<TConsideration, TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>([CanBeNull] TArg0 arg0, [CanBeNull] TArg1 arg1, [CanBeNull] TArg2 arg2, [CanBeNull] TArg3 arg3, [CanBeNull] TArg4 arg4, [CanBeNull] TArg5 arg5, [CanBeNull] TArg6 arg6)
			where TConsideration : Consideration, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>, new()
		{
            using (ConsiderationCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetConsiderationMarker<TConsideration>().Auto())
                {
#endif
                    var consideration = new TConsideration();
                    consideration.Setup(arg0, arg1, arg2, arg3, arg4, arg5, arg6);

                    return consideration;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates a consideration.
		/// </summary>
		/// <param name="arg0">First argument in a setup method.</param>
		/// <param name="arg1">Second argument in a setup method.</param>
		/// <param name="arg2">Third argument in a setup method.</param>
		/// <param name="arg3">Fourth argument in a setup method.</param>
		/// <param name="arg4">Fifth argument in a setup method.</param>
		/// <param name="arg5">Sixth argument in a setup method.</param>
		/// <param name="arg6">Seventh argument in a setup method.</param>
		/// <param name="arg7">Eighth argument in a setup method.</param>
		/// <typeparam name="TConsideration">Consideration type.</typeparam>
		/// <typeparam name="TArg0">First argument in a setup method type.</typeparam>
		/// <typeparam name="TArg1">Second argument in a setup method type.</typeparam>
		/// <typeparam name="TArg2">Third argument in a setup method type.</typeparam>
		/// <typeparam name="TArg3">Fourth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg4">Fifth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg5">Sixth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg6">Seventh argument in a setup method type.</typeparam>
		/// <typeparam name="TArg7">Eighth argument in a setup method type.</typeparam>
		/// <returns>Created consideration.</returns>
		[NotNull]
		public static TConsideration Create<TConsideration, TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>([CanBeNull] TArg0 arg0, [CanBeNull] TArg1 arg1, [CanBeNull] TArg2 arg2, [CanBeNull] TArg3 arg3, [CanBeNull] TArg4 arg4, [CanBeNull] TArg5 arg5, [CanBeNull] TArg6 arg6, [CanBeNull] TArg7 arg7)
			where TConsideration : Consideration, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>, new()
		{
            using (ConsiderationCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetConsiderationMarker<TConsideration>().Auto())
                {
#endif
                    var consideration = new TConsideration();
                    consideration.Setup(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

                    return consideration;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates a consideration.
		/// </summary>
		/// <param name="type">Consideration type. Must be derived from <see cref="Consideration"/>.</param>
		/// <returns>Created consideration.</returns>
		/// <remarks>
		/// This method doesn't call a setup method.
		/// </remarks>
		[NotNull]
		public static Consideration Create([NotNull] Type type)
		{
            using (ConsiderationCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetConsiderationMarker(type).Auto())
                {
#endif
                    var consideration = (Consideration)Activator.CreateInstance(type);

                    return consideration;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates a consideration.
		/// </summary>
		/// <param name="type">Consideration type. Must be derived from <see cref="Consideration"/>.</param>
		/// <param name="parameters">Setup method arguments. Must be up to 8 in length.</param>
		/// <returns>Created consideration.</returns>
		[NotNull]
		public static Consideration Create([NotNull] Type type, [NotNull, ItemCanBeNull] params object[] parameters)
		{
            using (ConsiderationCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetConsiderationMarker(type).Auto())
                {
#endif
                    var consideration = (Consideration)Activator.CreateInstance(type);
                    SetupableHelper.CreateSetup(consideration, parameters);

                    return consideration;
#if ENABLE_PROFILER
                }
#endif
            }
		}
	}
}
