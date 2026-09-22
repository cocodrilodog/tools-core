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
		public class State : DecisionStateBase<State, MonoDecisionStateMachine> { }

	}

}