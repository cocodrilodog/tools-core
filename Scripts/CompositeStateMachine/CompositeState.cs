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

}