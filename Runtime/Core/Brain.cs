// Copyright (c) 2023 Vladimir Popov zor1994@gmail.com https://github.com/ZorPastaman/UtilityAI

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Profiling;
using Zor.SimpleBlackboard.Core;
using Zor.UtilityAI.Debugging;

namespace Zor.UtilityAI.Core
{
	/// <summary>
	/// Brain of Utility AI.
	/// </summary>
	public sealed class Brain : IDisposable
	{
		private static readonly ProfilerMarker BrainInitializeMarker = new($"{nameof(Brain)}.{nameof(Initialize)}");
		private static readonly ProfilerMarker BrainTickMarker = new($"{nameof(Brain)}.{nameof(Tick)}");
		private static readonly ProfilerMarker BrainDisposeMarker = new($"{nameof(Brain)}.{nameof(Dispose)}");
		private static readonly ProfilerMarker BrainInitializeConsiderationsMarker = new($"{nameof(Brain)}.{nameof(InitializeConsiderations)}");
		private static readonly ProfilerMarker BrainInitializeActionsMarker = new($"{nameof(Brain)}.{nameof(InitializeActions)}");
		private static readonly ProfilerMarker BrainUpdateUtilitiesMarker = new($"{nameof(Brain)}.{nameof(UpdateUtilities)}");
		private static readonly ProfilerMarker BrainUpdateActionUtilitiesMarker = new($"{nameof(Brain)}.{nameof(UpdateActionUtilities)}");
		private static readonly ProfilerMarker BrainFindBestActionIndexMarker = new($"{nameof(Brain)}.{nameof(FindBestActionIndex)}");
		private static readonly ProfilerMarker BrainTrySwitchActionMarker = new($"{nameof(Brain)}.Try{nameof(SwitchAction)}");
		private static readonly ProfilerMarker BrainSwitchActionMarker = new($"{nameof(Brain)}.{nameof(SwitchAction)}");
		private static readonly ProfilerMarker BrainDisposeConsiderationsMarker = new($"{nameof(Brain)}.{nameof(DisposeConsiderations)}");
		private static readonly ProfilerMarker BrainDisposeActionsMarker = new($"{nameof(Brain)}.{nameof(DisposeActions)}");

		/// <summary>
		/// Considerations.
		/// </summary>
		[NotNull, ItemNotNull] private readonly Consideration[] m_considerations;
		/// <summary>
		/// Actions.
		/// </summary>
		[NotNull, ItemNotNull] private readonly Action[] m_actions;
		/// <summary>
		/// Blackboard.
		/// </summary>
		[NotNull] private readonly Blackboard m_blackboard;

		/// <summary>
		/// Considerations utilities. The arrays are bound by index.
		/// </summary>
		[NotNull] private readonly float[] m_utilities;
		/// <summary>
		/// Action utilities. The arrays are bound by index.
		/// </summary>
		[NotNull] private readonly float[] m_actionUtilities;
		/// <summary>
		/// Action to considerations bindings.
		/// </summary>
		[NotNull] private readonly int[][] m_actionConsiderationsBindings;

		/// <summary>
		/// Brain settings.
		/// </summary>
		private readonly BrainSettings m_brainSettings;

		/// <summary>
		/// Current action index.
		/// </summary>
		private int m_currentActionIndex;

		/// <summary>
		/// Creates a <see cref="Brain"/>.
		/// </summary>
		/// <param name="considerations">Considerations.</param>
		/// <param name="actions">Actions.</param>
		/// <param name="actionConsiderationsBindings">
		/// Action to considerations bindings.
		/// First array is bound to actions by index. Second array is bound to considerations by index.
		/// </param>
		/// <param name="blackboard">Used <see cref="Blackboard"/>.</param>
		/// <param name="brainSettings">Brain settings.</param>
		/// <remarks>
		/// <para>It may be much easier to create a brain with <see cref="Zor.UtilityAI.Builder.BrainBuilder"/>.</para>
		/// <para><paramref name="actionConsiderationsBindings"/> lengths must correspond
		/// to <paramref name="actions"/> and <paramref name="considerations"/> lengths.</para>
		/// </remarks>
		public Brain([NotNull, ItemNotNull] Consideration[] considerations, [NotNull, ItemNotNull] Action[] actions,
			[NotNull] int[][] actionConsiderationsBindings, [NotNull] Blackboard blackboard,
			BrainSettings brainSettings)
		{
			m_considerations = considerations;
			m_actions = actions;
			m_blackboard = blackboard;
			m_actionConsiderationsBindings = actionConsiderationsBindings;
			m_brainSettings = brainSettings;

			m_utilities = new float[m_considerations.Length];
			m_actionUtilities = new float[m_actions.Length];

			for (int i = 0, count = m_considerations.Length; i < count; ++i)
			{
				m_considerations[i].SetBlackboard(m_blackboard);
			}

			for (int i = 0, count = m_actions.Length; i < count; ++i)
			{
				m_actions[i].SetBlackboard(m_blackboard);
			}
		}

		/// <summary>
		/// <see cref="Blackboard"/> use by brain.
		/// </summary>
		[NotNull]
		public Blackboard blackboard
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
			get => m_blackboard;
		}

		/// <summary>
		/// Initializes a brain. It must be called only once and before first <see cref="Tick"/>.
		/// </summary>
		public void Initialize()
		{
			using (BrainInitializeMarker.Auto())
			{
                InitializeConsiderations();
                InitializeActions();
                UpdateUtilities();
                UpdateActionUtilities();
                m_currentActionIndex = FindBestActionIndex();
                m_actions[m_currentActionIndex].Begin();
			}
		}

		/// <summary>
		/// Ticks a brain.
		/// </summary>
		/// <remarks>
		/// <see cref="Initialize"/> must be called once before a first tick.
		/// </remarks>
		public void Tick()
		{
            using (BrainTickMarker.Auto())
            {
                UpdateUtilities();
                UpdateActionUtilities();
                int bestActionIndex = FindBestActionIndex();
                SwitchAction(bestActionIndex);
                m_actions[m_currentActionIndex].Tick();
            }
		}

		/// <summary>
		/// Disposes a brain.
		/// </summary>
		public void Dispose()
		{
            using (BrainDisposeMarker.Auto())
            {
                DisposeConsiderations();
                DisposeActions();
            }
		}

		/// <summary>
		/// Fills debug info into <paramref name="brainDebugInfo"/>.
		/// </summary>
		/// <param name="brainDebugInfo">Debug info.</param>
		/// <remarks>
		/// This method works in any build. It's written quite optimally.
		/// It's recommended to use the same <paramref name="brainDebugInfo"/> every call.
		/// This method can modify non-empty <paramref name="brainDebugInfo"/>.
		/// </remarks>
		public void FillDebugInfo([NotNull] BrainDebugInfo brainDebugInfo)
		{
			brainDebugInfo.brainSettings = m_brainSettings;

			int actionCount = m_actions.Length;

			List<BrainDebugInfo.ActionInfo> actionInfos = brainDebugInfo.actionInfos;
			for (int countDifference = actionCount - actionInfos.Count;
				countDifference > 0;
				--countDifference)
			{
				actionInfos.Add(new BrainDebugInfo.ActionInfo());
			}
			actionInfos.RemoveRange(actionCount, actionInfos.Count - actionCount);

			for (int actionIndex = 0; actionIndex < actionCount; ++actionIndex)
			{
				BrainDebugInfo.ActionInfo actionInfo = actionInfos[actionIndex];
				actionInfo.name = m_actions[actionIndex].name;
				actionInfo.utility = m_actionUtilities[actionIndex];
				actionInfo.isActive = m_currentActionIndex == actionIndex;

				int[] considerationBindings = m_actionConsiderationsBindings[actionIndex];
				int considerationCount = considerationBindings.Length;

				List<BrainDebugInfo.ConsiderationInfo> considerationInfos = actionInfo.considerationInfos;

				for (int countDifference = considerationCount - considerationInfos.Count;
					countDifference > 0;
					--countDifference)
				{
					considerationInfos.Add(new BrainDebugInfo.ConsiderationInfo());
				}
				considerationInfos.RemoveRange(considerationCount, considerationInfos.Count - considerationCount);

				for (int considerationIndex = 0; considerationIndex < considerationCount; ++considerationIndex)
				{
					int index = considerationBindings[considerationIndex];
					BrainDebugInfo.ConsiderationInfo considerationInfo = considerationInfos[considerationIndex];
					considerationInfo.name = m_considerations[index].name;
					considerationInfo.utility = m_utilities[index];
				}
			}
		}

		/// <summary>
		/// Initializes considerations.
		/// </summary>
		private void InitializeConsiderations()
		{
            using (BrainInitializeConsiderationsMarker.Auto())
            {
                for (int i = 0, count = m_considerations.Length; i < count; ++i)
                {
                    m_considerations[i].Initialize();
                }
            }
		}

		/// <summary>
		/// Initializes actions.
		/// </summary>
		private void InitializeActions()
		{
            using (BrainInitializeActionsMarker.Auto())
            {
                for (int i = 0, count = m_actions.Length; i < count; ++i)
                {
                    m_actions[i].Initialize();
                }
            }
		}

		/// <summary>
		/// Updates utilities of considerations. It calls <see cref="Consideration.ComputeUtility"/> in each element of
		/// <see cref="m_considerations"/> and puts the results into <see cref="m_utilities"/>.
		/// They are synced by index.
		/// </summary>
		private void UpdateUtilities()
		{
            using (BrainUpdateUtilitiesMarker.Auto())
            {
                for (int i = 0, count = m_utilities.Length; i < count; ++i)
                {
                    Consideration consideration = m_considerations[i];

#if ENABLE_PROFILER
                    using (Consideration.GetConsiderationMarker(consideration).Auto())
                    {
#endif
                        m_utilities[i] = consideration.ComputeUtility();
#if ENABLE_PROFILER
                    }
#endif
                }
            }
		}

		/// <summary>
		/// Updates utilities of actions. It multiplies utilities of all considerations of an action and
		/// puts the result into <see cref="m_actionUtilities"/>. That array and <see cref="m_actions"/>
		/// are synced by index.
		/// </summary>
		private void UpdateActionUtilities()
		{
            using (BrainUpdateActionUtilitiesMarker.Auto())
            {
                for (int i = 0, count = m_actionUtilities.Length; i < count; ++i)
                {
                    m_actionUtilities[i] = MultiplyUtilities(i);
                }
            }
		}

		/// <summary>
		/// Multiplies
		/// </summary>
		/// <param name="actionIndex"></param>
		/// <returns></returns>
		private float MultiplyUtilities(int actionIndex)
		{
			int[] utilityIndices = m_actionConsiderationsBindings[actionIndex];
			float utility = 1f;

			for (int i = 0, count = utilityIndices.Length; i < count; ++i)
			{
				utility *= m_utilities[utilityIndices[i]];
			}

			return utility;
		}

		/// <summary>
		/// Finds an action with the highest utility.
		/// </summary>
		/// <returns>Index of an action with the highest utility.</returns>
		private int FindBestActionIndex()
		{
            using (BrainFindBestActionIndexMarker.Auto())
            {
                int bestActionIndex = 0;
                float bestActionUtility = m_actionUtilities[bestActionIndex];

                for (int i = 1, count = m_actionUtilities.Length; i < count; ++i)
                {
                    float actionUtility = m_actionUtilities[i];

                    if (actionUtility > bestActionUtility)
                    {
                        bestActionIndex = i;
                        bestActionUtility = actionUtility;
                    }
                }

                return bestActionIndex;
            }
		}

		/// <summary>
		/// <para>Tries to switch to an action with the index <paramref name="newActionIndex"/>.</para>
		/// <para>The method checks if that index is different from <see cref="m_currentActionIndex"/>.
		/// It checks if a utility of a new action is higher than a utility of a current action
		/// by <see cref="BrainSettings.minimalUtilityDifference"/> at least.</para>
		/// <para>The method calls <see cref="Action.End"/> and <see cref="Action.Begin"/> if the switch happens.</para>
		/// </summary>
		/// <param name="newActionIndex">New action index.</param>
		private void SwitchAction(int newActionIndex)
		{
            using (BrainTrySwitchActionMarker.Auto())
            {
                if (m_currentActionIndex == newActionIndex)
                {
                    return;
                }

                float currentUtility = m_actionUtilities[m_currentActionIndex];
                float newUtility = m_actionUtilities[newActionIndex];

                if (newUtility - currentUtility < m_brainSettings.minimalUtilityDifference)
                {
                    return;
                }

                using (BrainSwitchActionMarker.Auto())
                {
                    m_actions[m_currentActionIndex].End();
                    m_currentActionIndex = newActionIndex;
                    m_actions[m_currentActionIndex].Begin();
                }
            }
		}

		/// <summary>
		/// Disposes considerations.
		/// </summary>
		private void DisposeConsiderations()
		{
            using (BrainDisposeConsiderationsMarker.Auto())
            {
                for (int i = 0, count = m_considerations.Length; i < count; ++i)
                {
                    m_considerations[i].Dispose();
                }
            }
		}

		/// <summary>
		/// Disposes actions
		/// </summary>
		private void DisposeActions()
		{
            using (BrainDisposeActionsMarker.Auto())
            {
                for (int i = 0, count = m_actions.Length; i < count; ++i)
                {
                    m_actions[i].Dispose();
                }
            }
		}
	}
}
