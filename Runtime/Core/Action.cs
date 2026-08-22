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
	/// Utility AI action.
	/// </summary>
	public abstract class Action
	{
        private static readonly ProfilerMarker ActionInitializeMarker = new($"{nameof(Action)}.{nameof(Initialize)}");
        private static readonly ProfilerMarker ActionBeginMarker = new($"{nameof(Action)}.{nameof(Begin)}");
        private static readonly ProfilerMarker ActionTickMarker = new($"{nameof(Action)}.{nameof(Tick)}");
        private static readonly ProfilerMarker ActionEndMarker = new($"{nameof(Action)}.{nameof(End)}");
        private static readonly ProfilerMarker ActionCreateMarker = new($"{nameof(Action)}.{nameof(Create)}");
        private static readonly ProfilerMarker ActionSetupMarker = new($"{nameof(Action)}.Setup");

#if ENABLE_PROFILER
        private static readonly ConcurrentDictionary<Type, ProfilerMarker> ActionMarkers = new();
        internal static ProfilerMarker GetActionMarker(Action consideration)
        {
            return GetActionMarker(consideration.GetType());
        }

        internal static ProfilerMarker GetActionMarker<T>()
            where T : Action
        {
            return GetActionMarker(typeof(T));
        }

        internal static ProfilerMarker GetActionMarker(Type type)
        {
            return ActionMarkers.GetOrAdd(type, static actionType => new ProfilerMarker(actionType.FullName));
        }
#endif

		/// <summary>
		/// Used <see cref="Blackboard"/>. Set via <see cref="Brain"/>.
		/// </summary>
		private Blackboard m_blackboard;

		/// <summary>
		/// Action's name. It's used for debugging mainly.
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
		/// The method is called once before a first tick of <see cref="Brain"/>.
		/// It's called for all actions even if they're inactive.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void OnInitialize() {}

		/// <summary>
		/// The method is called when the action becomes active.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void OnBegin() {}

		/// <summary>
		/// The method is called each tick of <see cref="Brain"/> if the action is active.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void OnTick() {}

		/// <summary>
		/// The method is called when the action becomes inactive.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void OnEnd() {}

		/// <summary>
		/// The method is called when <see cref="Brain"/> is disposed.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void OnDispose() {}

		/// <summary>
		/// Initializes an action.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Initialize()
		{
            using (ActionInitializeMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetActionMarker(this).Auto())
                {
#endif
                    OnInitialize();
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Begins an action.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Begin()
		{
            using (ActionBeginMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetActionMarker(this).Auto())
                {
#endif
                    OnBegin();
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Ticks an action.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Tick()
		{
            using (ActionTickMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetActionMarker(this).Auto())
                {
#endif
                    OnTick();
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Ends an action.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void End()
		{
            using (ActionEndMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetActionMarker(this).Auto())
                {
#endif
                    OnEnd();
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Disposes an action.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Dispose()
		{
#if ENABLE_PROFILER
            using (GetActionMarker(this).Auto())
            {
#endif
                OnDispose();
#if ENABLE_PROFILER
            }
#endif
		}

		/// <summary>
		/// Sets <see cref="Blackboard"/> into an action.
		/// </summary>
		/// <param name="blackboardToSet"><see cref="Blackboard"/> to set.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void SetBlackboard([NotNull] Blackboard blackboardToSet)
		{
			m_blackboard = blackboardToSet;
		}

		/// <summary>
		/// Creates an action.
		/// </summary>
		/// <typeparam name="TAction">Action type.</typeparam>
		/// <returns>Created action.</returns>
		[NotNull]
		public static TAction Create<TAction>() where TAction : Action, INotSetupable, new()
		{
            using (ActionCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetActionMarker<TAction>().Auto())
                {
#endif
                    var action = new TAction();

                    return action;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates an action.
		/// </summary>
		/// <param name="arg">Argument in a setup method.</param>
		/// <typeparam name="TAction">Action type.</typeparam>
		/// <typeparam name="TArg">Argument in a setup method type.</typeparam>
		/// <returns>Created action</returns>
		[NotNull]
		public static TAction Create<TAction, TArg>([CanBeNull] TArg arg) where TAction : Action, ISetupable<TArg>, new()
		{
            using (ActionCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetActionMarker<TAction>().Auto())
                {
#endif
                    var action = new TAction();

                    using (ActionSetupMarker.Auto())
                    {
                        action.Setup(arg);
                    }

                    return action;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates an action.
		/// </summary>
		/// <param name="arg0">First argument in a setup method.</param>
		/// <param name="arg1">Second argument in a setup method.</param>
		/// <typeparam name="TAction"></typeparam>
		/// <typeparam name="TArg0">First argument in a setup method type.</typeparam>
		/// <typeparam name="TArg1">Second argument in a setup method type.</typeparam>
		/// <returns>Created action.</returns>
		[NotNull]
		public static TAction Create<TAction, TArg0, TArg1>([CanBeNull] TArg0 arg0, [CanBeNull] TArg1 arg1)
			where TAction : Action, ISetupable<TArg0, TArg1>, new()
		{
            using (ActionCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetActionMarker<TAction>().Auto())
                {
#endif
                    var action = new TAction();

                    using (ActionSetupMarker.Auto())
                    {
                        action.Setup(arg0, arg1);
                    }

                    return action;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates an action.
		/// </summary>
		/// <param name="arg0">First argument in a setup method.</param>
		/// <param name="arg1">Second argument in a setup method.</param>
		/// <param name="arg2">Third argument in a setup method.</param>
		/// <typeparam name="TAction"></typeparam>
		/// <typeparam name="TArg0">First argument in a setup method type.</typeparam>
		/// <typeparam name="TArg1">Second argument in a setup method type.</typeparam>
		/// <typeparam name="TArg2">Third argument in a setup method type.</typeparam>
		/// <returns>Created action.</returns>
		[NotNull]
		public static TAction Create<TAction, TArg0, TArg1, TArg2>([CanBeNull] TArg0 arg0, [CanBeNull] TArg1 arg1, [CanBeNull] TArg2 arg2)
			where TAction : Action, ISetupable<TArg0, TArg1, TArg2>, new()
		{
            using (ActionCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetActionMarker<TAction>().Auto())
                {
#endif
                    var action = new TAction();

                    using (ActionSetupMarker.Auto())
                    {
                        action.Setup(arg0, arg1, arg2);
                    }

                    return action;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates an action.
		/// </summary>
		/// <param name="arg0">First argument in a setup method.</param>
		/// <param name="arg1">Second argument in a setup method.</param>
		/// <param name="arg2">Third argument in a setup method.</param>
		/// <param name="arg3">Fourth argument in a setup method.</param>
		/// <typeparam name="TAction"></typeparam>
		/// <typeparam name="TArg0">First argument in a setup method type.</typeparam>
		/// <typeparam name="TArg1">Second argument in a setup method type.</typeparam>
		/// <typeparam name="TArg2">Third argument in a setup method type.</typeparam>
		/// <typeparam name="TArg3">Fourth argument in a setup method type.</typeparam>
		/// <returns>Created action.</returns>
		[NotNull]
		public static TAction Create<TAction, TArg0, TArg1, TArg2, TArg3>([CanBeNull] TArg0 arg0, [CanBeNull] TArg1 arg1, [CanBeNull] TArg2 arg2, [CanBeNull] TArg3 arg3)
			where TAction : Action, ISetupable<TArg0, TArg1, TArg2, TArg3>, new()
		{
            using (ActionCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetActionMarker<TAction>().Auto())
                {
#endif
                    var action = new TAction();

                    using (ActionSetupMarker.Auto())
                    {
                        action.Setup(arg0, arg1, arg2, arg3);
                    }

                    return action;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates an action.
		/// </summary>
		/// <param name="arg0">First argument in a setup method.</param>
		/// <param name="arg1">Second argument in a setup method.</param>
		/// <param name="arg2">Third argument in a setup method.</param>
		/// <param name="arg3">Fourth argument in a setup method.</param>
		/// <param name="arg4">Fifth argument in a setup method.</param>
		/// <typeparam name="TAction"></typeparam>
		/// <typeparam name="TArg0">First argument in a setup method type.</typeparam>
		/// <typeparam name="TArg1">Second argument in a setup method type.</typeparam>
		/// <typeparam name="TArg2">Third argument in a setup method type.</typeparam>
		/// <typeparam name="TArg3">Fourth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg4">Fifth argument in a setup method type.</typeparam>
		/// <returns>Created action.</returns>
		[NotNull]
		public static TAction Create<TAction, TArg0, TArg1, TArg2, TArg3, TArg4>([CanBeNull] TArg0 arg0, [CanBeNull] TArg1 arg1, [CanBeNull] TArg2 arg2, [CanBeNull] TArg3 arg3, [CanBeNull] TArg4 arg4)
			where TAction : Action, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4>, new()
		{
            using (ActionCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetActionMarker<TAction>().Auto())
                {
#endif
                    var action = new TAction();

                    using (ActionSetupMarker.Auto())
                    {
                        action.Setup(arg0, arg1, arg2, arg3, arg4);
                    }

                    return action;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates an action.
		/// </summary>
		/// <param name="arg0">First argument in a setup method.</param>
		/// <param name="arg1">Second argument in a setup method.</param>
		/// <param name="arg2">Third argument in a setup method.</param>
		/// <param name="arg3">Fourth argument in a setup method.</param>
		/// <param name="arg4">Fifth argument in a setup method.</param>
		/// <param name="arg5">Sixth argument in a setup method.</param>
		/// <typeparam name="TAction"></typeparam>
		/// <typeparam name="TArg0">First argument in a setup method type.</typeparam>
		/// <typeparam name="TArg1">Second argument in a setup method type.</typeparam>
		/// <typeparam name="TArg2">Third argument in a setup method type.</typeparam>
		/// <typeparam name="TArg3">Fourth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg4">Fifth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg5">Sixth argument in a setup method type.</typeparam>
		/// <returns>Created action.</returns>
		[NotNull]
		public static TAction Create<TAction, TArg0, TArg1, TArg2, TArg3, TArg4, TArg5>([CanBeNull] TArg0 arg0, [CanBeNull] TArg1 arg1, [CanBeNull] TArg2 arg2, [CanBeNull] TArg3 arg3, [CanBeNull] TArg4 arg4, [CanBeNull] TArg5 arg5)
			where TAction : Action, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5>, new()
		{
            using (ActionCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetActionMarker<TAction>().Auto())
                {
#endif
                    var action = new TAction();

                    using (ActionSetupMarker.Auto())
                    {

                        action.Setup(arg0, arg1, arg2, arg3, arg4, arg5);
                    }

                    return action;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates an action.
		/// </summary>
		/// <param name="arg0">First argument in a setup method.</param>
		/// <param name="arg1">Second argument in a setup method.</param>
		/// <param name="arg2">Third argument in a setup method.</param>
		/// <param name="arg3">Fourth argument in a setup method.</param>
		/// <param name="arg4">Fifth argument in a setup method.</param>
		/// <param name="arg5">Sixth argument in a setup method.</param>
		/// <param name="arg6">Seventh argument in a setup method.</param>
		/// <typeparam name="TAction"></typeparam>
		/// <typeparam name="TArg0">First argument in a setup method type.</typeparam>
		/// <typeparam name="TArg1">Second argument in a setup method type.</typeparam>
		/// <typeparam name="TArg2">Third argument in a setup method type.</typeparam>
		/// <typeparam name="TArg3">Fourth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg4">Fifth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg5">Sixth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg6">Seventh argument in a setup method type.</typeparam>
		/// <returns>Created action.</returns>
		[NotNull]
		public static TAction Create<TAction, TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>([CanBeNull] TArg0 arg0, [CanBeNull] TArg1 arg1, [CanBeNull] TArg2 arg2, [CanBeNull] TArg3 arg3, [CanBeNull] TArg4 arg4, [CanBeNull] TArg5 arg5, [CanBeNull] TArg6 arg6)
			where TAction : Action, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>, new()
		{
            using (ActionCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetActionMarker<TAction>().Auto())
                {
#endif
                    var action = new TAction();

                    using (ActionSetupMarker.Auto())
                    {
                        action.Setup(arg0, arg1, arg2, arg3, arg4, arg5, arg6);
                    }

                    return action;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates an action.
		/// </summary>
		/// <param name="arg0">First argument in a setup method.</param>
		/// <param name="arg1">Second argument in a setup method.</param>
		/// <param name="arg2">Third argument in a setup method.</param>
		/// <param name="arg3">Fourth argument in a setup method.</param>
		/// <param name="arg4">Fifth argument in a setup method.</param>
		/// <param name="arg5">Sixth argument in a setup method.</param>
		/// <param name="arg6">Seventh argument in a setup method.</param>
		/// <param name="arg7">Eighth argument in a setup method.</param>
		/// <typeparam name="TAction"></typeparam>
		/// <typeparam name="TArg0">First argument in a setup method type.</typeparam>
		/// <typeparam name="TArg1">Second argument in a setup method type.</typeparam>
		/// <typeparam name="TArg2">Third argument in a setup method type.</typeparam>
		/// <typeparam name="TArg3">Fourth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg4">Fifth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg5">Sixth argument in a setup method type.</typeparam>
		/// <typeparam name="TArg6">Seventh argument in a setup method type.</typeparam>
		/// <typeparam name="TArg7">Eighth argument in a setup method type.</typeparam>
		/// <returns>Created action.</returns>
		[NotNull]
		public static TAction Create<TAction, TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>([CanBeNull] TArg0 arg0, [CanBeNull] TArg1 arg1, [CanBeNull] TArg2 arg2, [CanBeNull] TArg3 arg3, [CanBeNull] TArg4 arg4, [CanBeNull] TArg5 arg5, [CanBeNull] TArg6 arg6, [CanBeNull] TArg7 arg7)
			where TAction : Action, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>, new()
		{
            using (ActionCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetActionMarker<TAction>().Auto())
                {
#endif
                    var action = new TAction();

                    using (ActionSetupMarker.Auto())
                    {

                        action.Setup(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                    }

                    return action;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates an action.
		/// </summary>
		/// <param name="type">Action type. Must be derived from <see cref="Action"/>.</param>
		/// <returns>Created action.</returns>
		/// <remarks>
		/// This method doesn't call a setup method.
		/// </remarks>
		[NotNull]
		public static Action Create([NotNull] Type type)
		{
            using (ActionCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetActionMarker(type).Auto())
                {
#endif
                    var action = (Action)Activator.CreateInstance(type);

                    return action;
#if ENABLE_PROFILER
                }
#endif
            }
		}

		/// <summary>
		/// Creates an action.
		/// </summary>
		/// <param name="type">Action type. Must be derived from <see cref="Action"/>.</param>
		/// <param name="parameters">Setup method arguments. Must be up to 8 in length.</param>
		/// <returns>Created action.</returns>
		[NotNull]
		public static Action Create([NotNull] Type type, [NotNull, ItemCanBeNull] params object[] parameters)
		{
            using (ActionCreateMarker.Auto())
            {
#if ENABLE_PROFILER
                using (GetActionMarker(type).Auto())
                {
#endif
                    var action = (Action)Activator.CreateInstance(type);
                    SetupableHelper.CreateSetup(action, parameters);

                    return action;
#if ENABLE_PROFILER
                }
#endif
            }
		}
	}
}
