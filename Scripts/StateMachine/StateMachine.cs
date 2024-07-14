namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Events;

	public class StateMachine : StateMachineBase<StateMachine.State, StateMachine> {

		[Serializable]
		public class State : StateBase<State, StateMachine> { }

	}

}