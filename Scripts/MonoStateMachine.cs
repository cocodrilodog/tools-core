namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public abstract class MonoStateMachine<T_State, T_Machine> : MonoBehaviour
		where T_State : MonoState<T_State, T_Machine>
		where T_Machine : MonoStateMachine<T_State, T_Machine> {


		#region Protected Properties

		protected virtual T_State State {
			get { return m_State; }
			set {
				if(m_State != null) {
					m_State.Exit();
				}
				m_State = value;
				m_State.Enter();
			}
		}

		#endregion


		#region Private Fields

		[NonSerialized]
		private T_State m_State;

		#endregion


	}

	public abstract class MonoState<T_State, T_Machine>
		where T_State : MonoState<T_State, T_Machine>
		where T_Machine : MonoStateMachine<T_State, T_Machine> {


		#region Public Methods

		public abstract void Enter();

		public abstract void Exit();

		#endregion


		#region Protected Constructors

		protected MonoState(T_Machine machine) {
			m_Machine = machine;
		}

		#endregion


		#region Protected Properties

		protected T_Machine Machine { get { return m_Machine; } }

		#endregion


		#region Private Fields

		[NonSerialized]
		private T_Machine m_Machine;

		#endregion


	}

}