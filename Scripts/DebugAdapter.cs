namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class DebugAdapter : MonoBehaviour {


		#region Public Methods

		public void Log(string message) => Debug.Log(message);

		public void Log(bool message) => Debug.Log(message);

		public void Log(float message) => Debug.Log(message);
		
		public void Log(int message) => Debug.Log(message);

		#endregion


	}

}