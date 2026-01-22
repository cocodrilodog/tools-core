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

		public int TriggersCount => m_Triggers.Count;

		#endregion


		#region Public Methods

		public void NextState() {
			if (CurrentState == null) {
				Debug.LogWarning($"{name}: State is null. Did nothing.");
				return;
			}
			CurrentState.Next();
		}

		public void NextStateByIndex(int index) {
			if (CurrentState == null) {
				Debug.LogWarning($"{name}: State is null. Index: {index}. Did nothing.");
				return;
			}
			CurrentState.NextByIndex(index);
		}

		public void NextStateByTrigger(string trigger) {
			if (CurrentState == null) {
				Debug.LogWarning($"{name}: State is null. Trigger: {trigger}. Did nothing.");
				return;
			}
			CurrentState.NextByTrigger(trigger);
		}

		public void ForceStateByName(string name) => CurrentState.TransitionToState(name);

		public DecisionTrigger GetTriggerAtIndex(int index) => m_Triggers[index];

		public DecisionTrigger GetTriggerByName(string name) => m_Triggers.FirstOrDefault(t => t.Name == name);

		public void AddTrigger(string trigger) => AddTrigger(new DecisionTrigger(trigger));

		public void AddTrigger(DecisionTrigger trigger) {
			m_Triggers.Add(trigger);
			trigger.RegisterAsReferenceable(this);
		}

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

	[Serializable]
	public abstract class DecisionStateBase<T_State, T_Machine> : MonoCompositeState<T_State, T_Machine>
			where T_Machine : MonoDecisionStateMachineBase<T_State, T_Machine>
			where T_State : DecisionStateBase<T_State, T_Machine> {


		#region Public Properties

		public int NextOptionsCount => m_NextOptions.Count;

		#endregion


		#region Public Methods

		public DecisionOption<T_State, T_Machine> GetNextOptionAtIndex(int index) => m_NextOptions[index];

		public void AddNextOption(DecisionOption<T_State, T_Machine> nextOption) => m_NextOptions.Add(nextOption);

		public void RemoveNextOption(int index) => m_NextOptions.RemoveAt(index);

		public virtual bool Next() {
			return NextByIndex(0);
		}

		public virtual bool NextByIndex(int index) {
			if (index >= 0 && index < m_NextOptions.Count) { // Index is valid
				TransitionToState(m_NextOptions[index].State.Value.Name);
				return true;
			}
			return false;
		}

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

		[FormerlySerializedAs("m_NextStateOptions")]
		[SerializeField]
		private List<DecisionOption<T_State, T_Machine>> m_NextOptions = new();

		#endregion


	}

	/// <summary>
	/// Non-generic class to support custom property drawer.
	/// </summary>
	[Serializable]
	public class DecisionOption { }

	[Serializable]
	public class DecisionOption<T_State, T_Machine> : DecisionOption
		where T_Machine : MonoDecisionStateMachineBase<T_State, T_Machine>
		where T_State : DecisionStateBase<T_State, T_Machine> {


		#region Public Properties

		public DecisionTrigger Trigger => m_Trigger.Value;

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

}