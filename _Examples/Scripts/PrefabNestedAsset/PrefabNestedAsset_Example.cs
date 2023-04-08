namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class PrefabNestedAsset_Example : MonoBehaviour {


		private void Start() {
			m_NestedAsset.Log();
		}

		#region Private Fields

		[SerializeField]
		private GameObject m_Prefab;

		[SerializeField]
		private NestedAsset m_NestedAsset;

		#endregion


	}

}