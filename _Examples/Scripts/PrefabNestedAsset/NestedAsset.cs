namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class NestedAsset : ScriptableObject {


		public void Log() {
			Debug.Log($"m_MainObject: {m_MainObject}");
			Debug.Log($"m_ChildObject: {m_ChildObject}");
		}

		#region Private Fields

		[SerializeField]
		private GameObject m_MainObject;

		[SerializeField]
		private GameObject m_ChildObject;

		#endregion


	}

}