namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A template for <see cref="MonoBehaviour"/> state machines.
	/// </summary>
	/// <typeparam name="T_State">The type of the state.</typeparam>
	/// <typeparam name="T_Machine">The type of the state machine.</typeparam>
	public abstract class MonoStateMachine<T_State, T_Machine> : MonoBehaviour
		where T_State : MonoState<T_State, T_Machine>
		where T_Machine : MonoStateMachine<T_State, T_Machine> {


		#region Unity Methods

		protected virtual void Start() {
			State = DefaultState;
		}

		#endregion


		#region Protected Properties

		protected virtual T_State State {
			get {
				if(m_State == null) {
					State = DefaultState;
				}
				return m_State; 
			}
			set {
				if(m_State != null) {
					m_State.Exit();
				}
				m_State = value;
				m_State.Enter();
			}
		}

		protected abstract T_State DefaultState { get; }

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