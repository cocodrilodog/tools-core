namespace CocodriloDog.Core {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Base class for <see cref="MonoCompositeStateMachine"/> that transitions to states according to
	/// decisions. Each state can have a set of next options to choose from and it can transition to any
	/// of those state in three ways: 
	/// <list type="bullet">
	/// <item><description><c>NextState()</c>: To the first state in the list of next options.</description></item>
	/// <item><description><c>NextStateByIndex(int)</c>: To the state located at the provided index.</description></item>
	/// <item><description><c>NextStateByTrigger(string)</c>: To the state associated to the provided trigger.</description></item>
	/// </list>
	/// </summary>
	/// <typeparam name="T_State">The type of the state.</typeparam>
	/// <typeparam name="T_Machine">The type of the state machine.</typeparam>
	public abstract class MonoDecisionStateMachineBase<T_State, T_Machine> : MonoCompositeStateMachine<T_State, T_Machine>
		where T_Machine : MonoDecisionStateMachineBase<T_State, T_Machine>
		where T_State : DecisionStateBase<T_State, T_Machine> {


		#region Public Properties

		/// <summary>
		/// The count of <see cref="DecisionTrigger"/>s of this state machine.
		/// </summary>
		public int TriggersCount => m_Triggers.Count;

		#endregion


		#region Public Methods

		/// <summary>
		/// Transitions to the first state referenced in the 
		/// <see cref="DecisionStateBase{T_State, T_Machine}.m_NextOptions"/> list.
		/// </summary>
		public void NextState() {
			if (CurrentState == null) {
				Debug.LogWarning($"{name}: State is null. Did nothing.");
				return;
			}
			CurrentState.Next();
		}

		/// <summary>
		/// Transitions to the state at <paramref name="index"/> in the 
		/// <see cref="DecisionStateBase{T_State, T_Machine}.m_NextOptions"/> list.
		/// </summary>
		/// <param name="index">The index of the <see cref="DecisionOption{T_State, T_Machine}"/>.</param>
		public void NextStateByIndex(int index) {
			if (CurrentState == null) {
				Debug.LogWarning($"{name}: State is null. Index: {index}. Did nothing.");
				return;
			}
			CurrentState.NextByIndex(index);
		}

		/// <summary>
		/// Transitions to the state referenced by the <see cref="DecisionOption{T_State, T_Machine}"/>
		/// that has the provided <paramref name="trigger"/> in the 
		/// <see cref="DecisionStateBase{T_State, T_Machine}.m_NextOptions"/> list.
		/// </summary>
		/// <param name="trigger">The trigger.</param>
		public void NextStateByTrigger(string trigger) {
			if (CurrentState == null) {
				Debug.LogWarning($"{name}: State is null. Trigger: {trigger}. Did nothing.");
				return;
			}
			CurrentState.NextByTrigger(trigger);
		}

		/// <summary>
		/// Forecefully transitions to the state with <paramref name="name"/>, bypassing the 
		/// <c>NextState...()</c> logic.
		/// </summary>
		/// <param name="name"></param>
		public void ForceStateByName(string name) => CurrentState.TransitionToState(name);

		/// <summary>
		/// Gets the <see cref="DecisionTrigger"/> at <paramref name="index"/>.
		/// </summary>
		/// <param name="index">The index of the trigger.</param>
		/// <returns>The <see cref="DecisionTrigger"/>.</returns>
		public DecisionTrigger GetTriggerAtIndex(int index) => m_Triggers[index];

		/// <summary>
		/// Gets the <see cref="DecisionTrigger"/> with <paramref name="name"/>..
		/// </summary>
		/// <param name="name">The name of the trigger.</param>
		/// <returns>The <see cref="DecisionTrigger"/>.</returns>
		public DecisionTrigger GetTriggerByName(string name) => m_Triggers.FirstOrDefault(t => t.Name == name);

		/// <summary>
		/// adds a <see cref="DecisionTrigger"/>.
		/// </summary>
		/// <param name="trigger">The name of trigger to add.</param>
		public void AddTrigger(string trigger) => AddTrigger(new DecisionTrigger(trigger));

		/// <summary>
		/// adds a <see cref="DecisionTrigger"/>.
		/// </summary>
		/// <param name="trigger">The trigger to add.</param>
		public void AddTrigger(DecisionTrigger trigger) {
			m_Triggers.Add(trigger);
			trigger.RegisterAsReferenceable(this);
		}

		/// <summary>
		/// Removes the <see cref="DecisionTrigger"/> at the provided <paramref name="index"/>.
		/// </summary>
		/// <param name="index"></param>
		public void RemoveTrigger(int index) => m_Triggers.RemoveAt(index);

		#endregion


		#region Unity Methods

		protected override void Awake() {
			base.Awake();
			m_Triggers.ForEach(t => t.RegisterAsReferenceable(this));
		}

		protected override void OnValidate() {
			base.OnValidate();
			for (int i = 0; i < m_Triggers.Count; i++) {
				var trigger = m_Triggers[i];
				if (trigger == null) {
					m_Triggers[i] = new DecisionTrigger();
				}
				m_Triggers[i].CanDeleteInstance = false;
			}
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			m_Triggers.ForEach(t => t.UnregisterReferenceable(this));
		}

		#endregion


		#region Private Fields

		[FormerlySerializedAs("m_Triggers2")]
		[SerializeField]
		private CompositeList<DecisionTrigger> m_Triggers = new();

		#endregion


	}

	/// <summary>
	/// Base class for states for <see cref="MonoCompositeStateMachine{T_State, T_Machine}"/>
	/// </summary>
	/// <typeparam name="T_State">The type of <see cref="DecisionStateBase{T_State, T_Machine}"/> </typeparam>
	/// <typeparam name="T_Machine">The type of <see cref="MonoDecisionStateMachineBase{T_State, T_Machine}"/></typeparam>
	[Serializable]
	public abstract class DecisionStateBase<T_State, T_Machine> : MonoCompositeState<T_State, T_Machine>
			where T_Machine : MonoDecisionStateMachineBase<T_State, T_Machine>
			where T_State : DecisionStateBase<T_State, T_Machine> {


		#region Public Properties

		/// <summary>
		/// The count of <see cref="DecisionOption{T_State, T_Machine}"/>s that this state has.
		/// </summary>
		public int NextOptionsCount => m_NextOptions.Count;

		#endregion


		#region Public Methods

		/// <summary>
		/// Gets the <see cref="DecisionOption{T_State, T_Machine}"/> at the specified <paramref name="index"/>.
		/// </summary>
		/// <param name="index">The index of the <see cref="DecisionOption{T_State, T_Machine}"/></param>
		/// <returns>The <see cref="DecisionOption{T_State, T_Machine}"/>.</returns>
		public DecisionOption<T_State, T_Machine> GetNextOptionAtIndex(int index) => m_NextOptions[index];

		/// <summary>
		/// Adds a <see cref="DecisionOption{T_State, T_Machine}"/>.
		/// </summary>
		/// <param name="nextOption">The <see cref="DecisionOption{T_State, T_Machine}"/> to add.</param>
		public void AddNextOption(DecisionOption<T_State, T_Machine> nextOption) => m_NextOptions.Add(nextOption);

		/// <summary>
		/// Removes the <see cref="DecisionOption{T_State, T_Machine}"/> at <paramref name="index"/>.
		/// </summary>
		/// <param name="index"></param>
		public void RemoveNextOption(int index) => m_NextOptions.RemoveAt(index);

		/// <summary>
		/// Transitions to the state in the first <see cref="DecisionOption{T_State, T_Machine}"/>
		/// </summary>
		/// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
		public virtual bool Next() {
			return NextByIndex(0);
		}

		/// <summary>
		/// Transitions to the state in the <see cref="DecisionOption{T_State, T_Machine}"/> at the 
		/// specified <paramref name="index"/>.
		/// </summary>
		/// <param name="index">The index of the <see cref="DecisionOption{T_State, T_Machine}"/></param>
		/// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
		public virtual bool NextByIndex(int index) {
			if (index >= 0 && index < m_NextOptions.Count) { // Index is valid
				TransitionToState(m_NextOptions[index].State.Value.Name);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Transitions to the state in the <see cref="DecisionOption{T_State, T_Machine}"/> that
		/// has the specified <paramref name="trigger"/>.
		/// </summary>
		/// <param name="trigger">The trigger</param>
		/// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
		public virtual bool NextByTrigger(string trigger) {

			var stateOption = m_NextOptions.FirstOrDefault(o => o.Trigger?.Name == trigger);

			// Trigger is valid
			if (stateOption != null) {
				Debug.Log($"{Machine.name}: {Name} -> {trigger} -> {stateOption.State.Value.Name}");
				TransitionToState(stateOption.State.Value.Name);
				return true;
			}

			return false;

		}

		#endregion


		#region Unity Methods

		override public void OnValidate(UnityEngine.Object editModeMachine) {
			base.OnValidate(editModeMachine);
			m_NextOptions.ForEach(o => o.InitializeCompositeObjectReferences(editModeMachine, CompositeObjectReferenceMode.CannotChooseSource_HideSource));
		}

		#endregion


		#region Private Fields

		[Tooltip("The options for this state to transition to.")]
		[FormerlySerializedAs("m_NextStateOptions")]
		[SerializeField]
		private List<DecisionOption<T_State, T_Machine>> m_NextOptions = new();

		#endregion


	}

	// Non-generic class to support custom property drawer.
	[Serializable]
	public class DecisionOption { }

	/// <summary>
	/// An option for a <see cref="DecisionStateBase{T_State, T_Machine}"/> to transition to, when invoking
	/// <see cref="MonoDecisionStateMachineBase{T_State, T_Machine}.NextState"/>,
	/// <see cref="MonoDecisionStateMachineBase{T_State, T_Machine}.NextStateByIndex"/>,
	/// and <see cref="MonoDecisionStateMachineBase{T_State, T_Machine}.NextStateByTrigger"/>,
	/// </summary>
	/// <typeparam name="T_State">The type of <see cref="DecisionStateBase{T_State, T_Machine}"/> </typeparam>
	/// <typeparam name="T_Machine">The type of <see cref="MonoDecisionStateMachineBase{T_State, T_Machine}"/></typeparam>
	[Serializable]
	public class DecisionOption<T_State, T_Machine> : DecisionOption
		where T_Machine : MonoDecisionStateMachineBase<T_State, T_Machine>
		where T_State : DecisionStateBase<T_State, T_Machine> {


		#region Public Properties

		/// <summary>
		/// The <see cref="DecisionTrigger"/> that will transition to the specified <see cref="State"/>
		/// when invoking <see cref="MonoDecisionStateMachineBase{T_State, T_Machine}.NextStateByTrigger"/>.
		/// </summary>
		public DecisionTrigger Trigger => m_Trigger.Value;

		/// <summary>
		/// A reference to the <see cref="DecisionStateBase{T_State, T_Machine}"/> that this option
		/// will transition to.
		/// </summary>
		public CompositeObjectReference<T_State> State => m_State;

		public string TriggerId {
			get => m_Trigger.Id;
			set => m_Trigger.Id = value;
		}

		public string StateId {
			get => m_State.Id;
			set => m_State.Id = value;
		}

		#endregion



		#region Public Constructors

		public DecisionOption(T_Machine machine) {
			SetSource(machine);
			SetMode(CompositeObjectReferenceMode.CannotChooseSource_HideSource);
		}

		#endregion


		#region Public Methods

		public void InitializeCompositeObjectReferences(UnityEngine.Object source, CompositeObjectReferenceMode mode) {
			SetSource(source);
			SetMode(mode);
		}

		#endregion


		#region Private Fields

		[SerializeField]
		private CompositeObjectReference<DecisionTrigger> m_Trigger = new();

		[SerializeField]
		private CompositeObjectReference<T_State> m_State = new();

		#endregion


		#region Private Constructors

		private DecisionOption() { }

		#endregion


		#region Private Methods

		private void SetSource(UnityEngine.Object source) {
			m_State.Source = source;
			m_Trigger.Source = source;
		}

		private void SetMode(CompositeObjectReferenceMode mode) {
			m_State.SetMode(mode);
			m_Trigger.SetMode(mode);
		}

		#endregion


	}

	/// <summary>
	/// The <see cref="DecisionOption{T_State, T_Machine}.Trigger"/> to be used in 
	/// <see cref="MonoDecisionStateMachineBase{T_State, T_Machine}.NextStateByTrigger(string)"/>
	/// to transition to the corresponding <see cref="DecisionOption{T_State, T_Machine}.State"/>.
	/// </summary>
	[Serializable]
	public class DecisionTrigger : CompositeObject {


		#region Public Properties

		public override string DisplayName => $"{base.DisplayName} (Trigger)";

		#endregion


		#region Public Constructors

		public DecisionTrigger() { }

		public DecisionTrigger(string name) => Name = name;

		#endregion


	}

}