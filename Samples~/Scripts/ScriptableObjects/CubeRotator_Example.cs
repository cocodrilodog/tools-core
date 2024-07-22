namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class CubeRotator_Example : MonoBehaviour {


		#region Unity Methods

		private void Update() {
			m_Cube.Value.Rotate(0, m_RotationSpeed.Value * Time.deltaTime, 0);
		}

		#endregion


		#region Private Fields

		[Tooltip("A cube transform.")]
		[SerializeField]
		private ScriptableReferenceGetter<Transform> m_Cube;

		[Tooltip("The speed of the rotation.")]
		[SerializeField]
		private ScriptableValueGetter<float> m_RotationSpeed;

		#endregion


	}

}