namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Triggers events on Awake(), Start(), OnEnable(), OnDisable(), OnDestroy().
	/// </summary>
	public class MonoLifeCycleTrigger : MonoBehaviour {


		#region Public Events

		public event Action OnAwakeEv;		// added Ev for consistency

		public event Action OnStartEv;		// added Ev for consistency

		public event Action OnDestroyEv;	// added Ev to break the conflict with OnDestroy

		public event Action OnEnableEv;		// added Ev to break the conflict with OnEnable

		public event Action OnDisableEv;	// added Ev to break the conflict with OnDisable

		#endregion


		#region Unity Methods

		private void Awake() {
			OnAwakeEv?.Invoke();
			m_OnAwake.Invoke();
		}

		private void OnEnable() {
			OnEnableEv?.Invoke();
			m_OnEnable.Invoke();
		}

		private void Start() {
			OnStartEv?.Invoke();
			m_OnStart.Invoke();
		}

		private void OnDisable() {
			OnDisableEv?.Invoke();
			m_OnDisable.Invoke();
		}

		private void OnDestroy() {

			OnDestroyEv?.Invoke();
			m_OnDestroy.Invoke();

			OnAwakeEv = null;
			OnStartEv = null;
			OnDestroyEv = null;
			OnEnableEv = null;
			OnDisableEv = null;

		}

		#endregion


		#region Private Fields

		[UnityEventGroup("Main")]
		[SerializeField]
		private UnityEvent m_OnAwake;

		[UnityEventGroup("Main")]
		[SerializeField]
		private UnityEvent m_OnStart;

		[UnityEventGroup("Main")]
		[SerializeField]
		private UnityEvent m_OnDestroy;

		[UnityEventGroup("EnableDisable")]
		[SerializeField]
		private UnityEvent m_OnEnable;

		[UnityEventGroup("EnableDisable")]
		[SerializeField]
		private UnityEvent m_OnDisable;

		#endregion


	}

}
