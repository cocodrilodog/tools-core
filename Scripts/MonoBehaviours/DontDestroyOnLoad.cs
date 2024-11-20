namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Causes this game object not to be destroyed when a new scene loads.
	/// </summary>
	public class DontDestroyOnLoad : MonoBehaviour {


		#region Unity Methods

		private void Awake() => DontDestroyOnLoad(gameObject);

		#endregion


	}

}