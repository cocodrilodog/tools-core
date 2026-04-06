namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Concrete immplementation of <see cref="MonoDecisionStateMachineBase{T_State, T_Machine}"/>,
	/// with states that can be timed.
	/// </summary>
	public class MonoDecisionStateMachine : MonoDecisionStateMachineBase<MonoDecisionStateMachine.State, MonoDecisionStateMachine> {


		[Serializable]
		public class State : DecisionStateBase<State, MonoDecisionStateMachine> {


			#region Public Methods

			public override void Enter() {
				base.Enter();
				if (m_IsTimed) {
					m_DurationCoroutine = Machine.StartCoroutine(WaitForDuration());
				}
				IEnumerator WaitForDuration() {
					yield return new WaitForSeconds(m_Duration);
					Next();
				}
			}

			public override void Exit() {
				base.Exit();
				if (m_DurationCoroutine != null) {
					Machine.StopCoroutine(m_DurationCoroutine);
					m_DurationCoroutine = null;
				}
			}

			#endregion


			#region Private Fields - Serialized

			[SerializeField]
			private bool m_IsTimed;

			[Hide(nameof(HideDuration))]
			[SerializeField]
			private float m_Duration;

			#endregion


			#region Private Fields - Non Serialized

			[NonSerialized]
			private Coroutine m_DurationCoroutine;

			#endregion


			#region Private Methods

			private bool HideDuration() => !m_IsTimed;

			#endregion


		}

	}

}