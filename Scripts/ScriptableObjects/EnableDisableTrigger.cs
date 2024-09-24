namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Triggers events when it is enabled or disabled.
	/// </summary>
	public class EnableDisableTrigger : MonoBehaviour {


		#region Unity Methods

		private void OnEnable() => OnEnableUE.Invoke();

		private void OnDisable() => OnDisableUE.Invoke();

		#endregion


		#region Private Fields

		[UnityEventGroup("Events")]
		[SerializeField]
		private UnityEvent OnEnableUE;

		[UnityEventGroup("Events")]
		[SerializeField]
		private UnityEvent OnDisableUE;

		#endregion


	}

}
