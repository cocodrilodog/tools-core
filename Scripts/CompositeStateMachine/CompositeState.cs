namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Intermediate non-generic class created to support a base property drawer for all subclasses.
	/// </summary>
	[Serializable]
	public class CompositeState : CompositeObject {


		#region Public Methods

		public virtual void Enter() { }

		public virtual void Exit() { }

		public void EnterAndRaiseOnEnter() {
			Enter();
			RaiseOnEnter();
		}

		public void ExitAndRaiseOnExit() {
			Exit();
			RaiseOnExit();
		}

		#endregion


		#region Unity Methods

		public virtual void Update() { }

		public virtual void FixedUpdate() { }

		public virtual void OnValidate() { }

		public virtual void OnValidate(UnityEngine.Object editModeMachine) { }

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


		#region Private Methods

		private void RaiseOnEnter() {
			OnEnter?.Invoke();
			m_OnEnter.Invoke();
		}

		private void RaiseOnExit() {
			OnExit?.Invoke();
			m_OnExit.Invoke();
		}

		#endregion


	}

}