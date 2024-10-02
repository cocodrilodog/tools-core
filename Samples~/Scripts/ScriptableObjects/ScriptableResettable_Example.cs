namespace CocodriloDog.Core.Examples {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class ScriptableResettable_Example : MonoBehaviour {


		#region Unity Methods

		private void Start() { 
			foreach(var so in m_ScriptableEditStateSOs) {
				// Change the values at runtime, and observe how they reset in edit mode
				so.SomeString = Guid.NewGuid().ToString();
				so.SomeFloat = UnityEngine.Random.Range(-1000, 1000);
				so.SomePosition = UnityEngine.Random.insideUnitSphere;
				so.Another = null;
			}
		}

		#endregion


		#region Private Fields

		[SerializeField]
		private List<ScriptableResettableSO_Example> m_ScriptableEditStateSOs;

		#endregion


	}

}