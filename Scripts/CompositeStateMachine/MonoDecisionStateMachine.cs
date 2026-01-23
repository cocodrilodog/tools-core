namespace CocodriloDog.Core {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Concrete immplementation of <see cref="MonoDecisionStateMachineBase{T_State, T_Machine}"/>.
	/// </summary>
	public class MonoDecisionStateMachine : MonoDecisionStateMachineBase<MonoDecisionStateMachine.State, MonoDecisionStateMachine> {


		#region States

		[Serializable]
		public class State : DecisionStateBase<State, MonoDecisionStateMachine> { }

		#endregion


	}

}