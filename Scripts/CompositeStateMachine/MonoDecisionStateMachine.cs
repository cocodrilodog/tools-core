namespace CocodriloDog.Core {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Serialization;

	public class MonoDecisionStateMachine : MonoDecisionStateMachineBase<MonoDecisionStateMachine.State, MonoDecisionStateMachine> {


		#region States

		[Serializable]
		public class State : DecisionStateBase<State, MonoDecisionStateMachine> { }

		#endregion


	}

}