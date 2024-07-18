namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// A state machine that transitions to the different states if the current state has a transition
	/// state to the target state defined in the <see cref="State.m_Transitions"/> list.
	/// </summary>
	public class FlowStateMachine : StateMachineBase<FlowStateMachine.State, FlowStateMachine> {


		#region Public Methods

		/// <summary>
		/// Attemps to transition to a state named <paramref name="name"/>.
		/// </summary>
		/// <param name="name"></param>
		public void SetState(string name) {
			CurrentState.TransitionToState(name);
		}

		#endregion


		#region States

		/// <summary>
		/// The state class for the <see cref="FlowStateMachine"/>.
		/// </summary>
		[Serializable]
		public class State : StateBase<State, FlowStateMachine> {

			public override void Enter() {
				if (m_Duration > 0) {
					m_TimerCoroutine = Machine.StartCoroutine(Timer());
					IEnumerator Timer() {
						yield return new WaitForSeconds(m_Duration);
						if (m_Transitions.Count > 0) {
							TransitionToState(m_Transitions[0]);
						}
					}
				}
			}

			public override void Exit() {
				if (m_TimerCoroutine != null) {
					Machine.StopCoroutine(m_TimerCoroutine);
				}
			}

			public override void TransitionToState(string name) {
				if (name == Name) {
					return;
				}
				if (!Machine.HasState(name)) {
					Debug.LogWarning($"Has no state with name {name}");
					return;
				}
				if (m_Transitions.Contains(name)) {
					Machine.SetState(Machine.States.FirstOrDefault(s => s.Name == name));
				} else {
					Debug.LogWarning($"{Machine.name}: Has no transition from {Name} to {name}");
				}
			}

			[Tooltip(
				"An optional duration of this state. After this time, the state machine will transition " +
				"to the first state on the transitions list. If left as 0, it won't do anything."
			)]
			[SerializeField]
			private float m_Duration;

			[Tooltip("The states that this state can transition to.")]
			[StringOptions("GetStateNames")]
			[SerializeField]
			private List<string> m_Transitions;

			[NonSerialized]
			private Coroutine m_TimerCoroutine;

		}

		#endregion


	}

}