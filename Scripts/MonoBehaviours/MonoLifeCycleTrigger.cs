namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Triggers events on Awake(), Start(), OnEnable(), OnDisable(), OnDestroy().
	/// </summary>
	public class MonoLifeCycleTrigger : MonoBehaviour {


		#region Unity Methods

		private void Awake() => OnAwake.Invoke();

		private void OnEnable() => OnEnableUE.Invoke();

		private void Start() => OnStart.Invoke();

		private void OnDisable() => OnDisableUE.Invoke();

		private void OnDestroy() => OnDestroyUE.Invoke();

		#endregion


		#region Private Fields

		[UnityEventGroup("Main")]
		[SerializeField]
		private UnityEvent OnAwake;

		[UnityEventGroup("Main")]
		[SerializeField]
		private UnityEvent OnStart;

		[UnityEventGroup("Main")]
		[SerializeField]
		private UnityEvent OnDestroyUE;

		[UnityEventGroup("EnableDisable")]
		[SerializeField]
		private UnityEvent OnEnableUE;

		[UnityEventGroup("EnableDisable")]
		[SerializeField]
		private UnityEvent OnDisableUE;

		#endregion


	}

}
