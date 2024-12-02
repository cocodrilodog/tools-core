namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Intermediate non-generic class created to support a base editor for all subclasses of
	/// <see cref="MonoCompositeStateMachine{T_State, T_Machine}"/>.
	/// </summary>
	public abstract class MonoCompositeStateMachine : MonoCompositeRoot { }

	/// <summary>
	/// Base class to create state machines that use <see cref="CompositeObject"/>s as base class for
	/// their corresponding states.
	/// </summary>
	/// <typeparam name="T_State">The type of the state</typeparam>
	/// <typeparam name="T_Machine">The type of the state machine</typeparam>
	public abstract class MonoCompositeStateMachine<T_State, T_Machine> : MonoCompositeStateMachine
		where T_Machine : MonoCompositeStateMachine<T_State, T_Machine>
		where T_State : CompositeState<T_State, T_Machine> {


		#region Public Properties

		/// <summary>
		/// Used to enable/disable the ability to add or remove states in the inspector.
		/// </summary>
		/// <remarks>
		/// This should be called from <c>OnValidate</c>
		/// </remarks>
		public bool CanAddRemoveStates {
			get => m_States.CanAddRemove;
			set => m_States.CanAddRemove = value;
		}

		/// <summary>
		/// Used to enable/disable the ability to reorder states in the inspector.
		/// </summary>
		/// <remarks>
		/// This should be called from <c>OnValidate</c>
		/// </remarks>
		public bool CanReorderStates {
			get => m_States.CanReorder;
			set => m_States.CanReorder = value;
		}

		/// <summary>
		/// The number of states.
		/// </summary>
		public int StatesCount => m_States.Count;

		#endregion


		#region Public Methods

		/// <summary>
		/// Checks whether a state with the provided <paramref name="name"/> exists or not.
		/// </summary>
		/// <param name="name">The name of the state.</param>
		/// <returns>
		/// <c>true</c> if a state with the specified <paramref name="name"/> exists, <c>false</c>
		/// otherwise.
		/// </returns>
		public bool HasState(string name) {
			if(m_States.FirstOrDefault(s => name == s.Name) != null) {
				return true;
			}
			return false; 
		}

		/// <summary>
		/// Gets the state at the specified <paramref name="index"/> or null if there is none.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The state.</returns>
		public T_State GetState(int index) => index >= 0 && index < m_States.Count ? m_States[index] : null;

		/// <summary>
		/// Gets the state that has the specified <paramref name="name"/> or null if there is none.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The state.</returns>
		public T_State GetState(string name) => m_States.FirstOrDefault(s => name == s.Name);

		/// <summary>
		/// Gets the index of the specified <paramref name="state"/>
		/// </summary>
		/// <typeparam name="T">The type of the state.</typeparam>
		/// <param name="state">The state.</param>
		/// <returns></returns>
		public int IndexOfState<T>(T state) where T : T_State {
			if (state == null) {
				throw new ArgumentNullException(nameof(state), "The provided state can not be null");
			}
			if (!m_States.Contains(state)) {
				throw new ArgumentException("The provided state is not on this state machine", nameof(state));
			}
			return m_States.IndexOf(state);
		}

		/// <summary>
		/// Iterates through the list of states and invokes an <paramref name="action"/> for each one.
		/// </summary>
		/// <param name="action">The action.</param>
		public void ForEachState(Action<T_State> action) {
			foreach(var state in m_States) {
				action?.Invoke(state);
			}
		}

		/// <summary>
		/// Gets a list with the names of the states.
		/// </summary>
		/// <returns></returns>
		public List<string> GetStateNames() {
			var names = new List<string>();
			ForEachState(s => names.Add(s.Name));
			return names;
		}

		#endregion


		#region Unity Methods

		protected virtual void Start() => SetState(m_States[0]);

		protected virtual void Update() => m_CurrentState.Update();

		protected virtual void FixedUpdate() => m_CurrentState.FixedUpdate();

		protected virtual void OnDestroy() {
			SetState(null); // This will exit the current state
			ForEachState(s => s.OnDestroy());
		}

		#endregion


		#region Protected Properties

		/// <summary>
		/// Returns the current active state.
		/// </summary>
		protected T_State CurrentState {
			get {
				Initialize();
				return m_CurrentState;
			}
		}

		#endregion


		#region Protected Methods

		/// <summary>
		/// Adds a state of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		protected void AddState<T>() where T : T_State => AddState(typeof(T));

		/// <summary>
		/// Adds a state of type <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type.</param>
		protected void AddState(Type type) {
			T_State state = Activator.CreateInstance(type) as T_State;
			m_States.Add(state);
			state.SetMachine(this as T_Machine);
		}

		/// <summary>
		/// Sets a state of type <typeparamref name="T"/> at the specified <paramref name="index"/> if there is none.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="index">The index.</param>
		protected void SetStateIfNull<T>(int index) where T : T_State => SetStateIfNull(typeof(T), index);

		/// <summary>
		/// Sets a state of type <paramref name="type"/> at the specified <paramref name="index"/> if there is none.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="index">The index.</param>
		protected void SetStateIfNull(Type type, int index) {
			if (GetState(index) == null) {
				SetState(type, index);
			}
		}

		/// <summary>
		/// Sets a state of type <typeparamref name="T"/> at the specified <paramref name="index"/>.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="index">The index.</param>
		protected void SetState<T>(int index) where T : T_State => SetState(typeof(T), index);

		/// <summary>
		/// Sets a state of type <paramref name="type"/> at the specified <paramref name="index"/>.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="index">The index.</param>
		protected void SetState(Type type, int index) {
			T_State state = Activator.CreateInstance(type) as T_State;
			while (index > m_States.Count - 1) {
				m_States.Add(null);
			}
			m_States[index] = state;
			state.SetMachine(this as T_Machine);
		}

		#endregion


		#region Internal Properties

		/// <summary>
		/// The list of states.
		/// </summary>
		internal CompositeList<T_State> States => m_States;

		/// <summary>
		/// Sets an active state.
		/// </summary>
		/// <remarks>
		/// The satte machine will set the first state on <see cref="Start"/>.
		/// </remarks>
		/// <param name="value">The state.</param>
		internal void SetState(T_State value) {
			Initialize();
			if (value != m_CurrentState) {
				m_CurrentState?.Exit();
				m_CurrentState?.RaiseOnExit();
				m_CurrentState = value;
				m_CurrentState?.Enter();
				m_CurrentState?.RaiseOnEnter();
			}
		}

		#endregion


		#region Private Fields - Serialized

		[SerializeField]
		private CompositeList<T_State> m_States = new CompositeList<T_State>();

		#endregion


		#region Private Fields - Non Serialized

		[NonSerialized]
		private bool m_IsInitialized;

		[NonSerialized]
		private T_State m_CurrentState;

		#endregion


		#region Private Methods

		private void Initialize() {
			if (!m_IsInitialized) {
				m_IsInitialized = true;
				foreach (var state in m_States) {
					state.SetMachine(this as T_Machine);
				}
			}
		}

		#endregion


	}

	/// <summary>
	/// Intermediate non-generic class created to support a base property drawer for all subclasses.
	/// </summary>
	[Serializable]
	public class CompositeState : CompositeObject {


		#region Public Methods

		public virtual void Enter() { }

		public void RaiseOnEnter() {
			OnEnter?.Invoke();
			m_OnEnter.Invoke();
		}

		public virtual void Exit() { }

		public void RaiseOnExit() {
			OnExit?.Invoke();
			m_OnExit.Invoke();
		}

		#endregion


		#region Unity Methods

		public virtual void Update() { }

		public virtual void FixedUpdate() { }

		public virtual void OnDestroy() {
			OnEnter = null;
			OnExit = null;
		}

		#endregion


		#region Public Events

		public event Action OnEnter;

		public event Action OnExit;

		#endregion


		#region Private Fields - Serialized

		[UnityEventGroup("Events")]
		[SerializeField]
		private UnityEvent m_OnEnter;

		[UnityEventGroup("Events")]
		[SerializeField]
		private UnityEvent m_OnExit;

		#endregion


	}

	[Serializable]
	public class CompositeState<T_State, T_Machine> : CompositeState
				where T_Machine : MonoCompositeStateMachine<T_State, T_Machine>
				where T_State : CompositeState<T_State, T_Machine> {


		#region Public Methods

		public virtual void TransitionToState(string name) {
			if (name == Name) {
				return;
			}
			if (!Machine.HasState(name)) {
				Debug.LogWarning($"Has no state with name {name}");
				return;
			}
			// Wait for one frame to handle edge case:
			// - Certain state may transition to another one on Enter()
			// - If that Enter() calls TransitionToState and the code is executed immediatly,
			// another Exit() and Enter() will be called before the original m_OnEnter event
			// is fired, potentially breaking the logic.
			Machine.StartCoroutine(Transition());
			IEnumerator Transition() {
				yield return null;
				Machine.SetState(Machine.States.FirstOrDefault(s => s.Name == name));
			}
		}

		#endregion


		#region Protected Properties

		protected T_Machine Machine => m_Machine;

		#endregion


		#region Internal Methods

		internal void SetMachine(T_Machine machine) => m_Machine = machine;

		#endregion


		#region Private Fields - Non Serialized

		[NonSerialized]
		private T_Machine m_Machine;

		#endregion


	}

}