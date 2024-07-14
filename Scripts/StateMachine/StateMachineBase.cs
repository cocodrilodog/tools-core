namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Events;

	public class StateMachineBase<T_State, T_Machine> : CompositeRoot 
		where T_Machine : StateMachineBase<T_State, T_Machine>
		where T_State : StateBase<T_State, T_Machine> {


		#region Public Methods

		public virtual void SetState(string name) {
			Initialize();
			m_State.SetState(name);
		}

		#endregion


		#region Unity Methods

		protected virtual void Start() {
			Initialize();
			SetState(m_States[0]);
		}

		#endregion


		#region Internal Properties

		internal CompositeList<T_State> States => m_States;

		internal void SetState(T_State value) {
			if (value != m_State) {
				m_State?.Exit();
				m_State?.RaiseOnExit();
				m_State = value;
				m_State?.Enter();
				m_State?.RaiseOnEnter();
			}
		}

		#endregion


		#region Protceted Methods

		protected virtual void Update() => m_State.Update();

		protected virtual void FixedUpdate() => m_State.Update();

		#endregion


		#region Private Fields - Serialized

		[SerializeField]
		private StringOptions m_NameOptions;

		[SerializeField]
		private CompositeList<T_State> m_States;

		#endregion


		#region Private Fields - Non Serialized

		[NonSerialized]
		private bool m_IsInitialized;

		[NonSerialized]
		private T_State m_State;

		#endregion


		#region Private Methods

		private void Initialize() {
			if (!m_IsInitialized) {
				m_IsInitialized = true;
				foreach (var state in m_States) {
					state.Initialize(this as T_Machine);
				}
			}
		}

		#endregion


	}

	[Serializable]
	public class StateBase<T_State, T_Machine> : CompositeObject
				where T_Machine : StateMachineBase<T_State, T_Machine>
				where T_State : StateBase<T_State, T_Machine> {


		#region Public Methods

		public void Initialize(T_Machine machine) {
			m_Machine = machine;
		}

		public virtual void Enter() {
			if (m_Duration > 0) {
				m_TimerCoroutine = Machine.StartCoroutine(Timer());
				IEnumerator Timer() {
					yield return new WaitForSeconds(m_Duration);
					if (m_Transitions.Count > 0) {
						SetState(m_Transitions[0]);
					}
				}
			}
		}

		public void RaiseOnEnter() => m_OnEnter.Invoke();

		public virtual void Exit() {
			if (m_TimerCoroutine != null) {
				Machine.StopCoroutine(m_TimerCoroutine);
			}
		}

		public void RaiseOnExit() => m_OnExit.Invoke();

		public virtual void SetState(string name) {
			if (name == Name) {
				return;
			}
			if (m_Transitions.Count > 0) {
				if (m_Transitions.Contains(name)) {
					Machine.SetState(Machine.States.FirstOrDefault(s => s.Name == name));
				} else {
					Debug.LogWarning($"{Machine.name}: Has no transition from {Name} to {name}");
				}
			} else {
				Machine.SetState(Machine.States.FirstOrDefault(s => s.Name == name));
			}
		}

		public virtual void Update() { }

		public virtual void FixedUpdate() { }

		#endregion


		#region Protected Properties

		protected T_Machine Machine => m_Machine;

		protected List<string> Transitions => m_Transitions;

		#endregion


		#region Private Fields - Serialized

		[SerializeField]
		public float m_Duration;

		[StringOptions("m_NameOptions")]
		[SerializeField]
		private List<string> m_Transitions;

		[UnityEventGroup("Events")]
		[SerializeField]
		private UnityEvent m_OnEnter;

		[UnityEventGroup("Events")]
		[SerializeField]
		private UnityEvent m_OnExit;

		#endregion


		#region Private Fields - Non Serialized

		[NonSerialized]
		private T_Machine m_Machine;

		[NonSerialized]
		private Coroutine m_TimerCoroutine;

		#endregion


	}

}