namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <inheritdoc cref="NonSceneCubeController_Example"/>
	public class NonSceneObjectController_Example : MonoBehaviour {


		#region Private Fields

		[SerializeField]
		private NonSceneCubeController_Example m_NonSceneCubePrefab;

		#endregion


		#region private Unity Methods

		private void Awake() => Instantiate(m_NonSceneCubePrefab);

		#endregion


	}

}