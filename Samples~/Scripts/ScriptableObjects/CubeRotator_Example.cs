namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class CubeRotator_Example : MonoBehaviour {


		#region Unity Methods

		private void OnEnable() {
			m_Cube.OnValueChange += Cube_OnValueChange;
			m_RotationSpeed.OnValueChange += RotationSpeed_OnValueChange;
		}

		private IEnumerator Start() {

			yield return new WaitForSeconds(1);
			var cubeValue = m_Cube.Value;
			var rotationSpeedValue = m_RotationSpeed.Value;
			m_Cube.Value = null;
			m_RotationSpeed.Value = 100;

			yield return new WaitForSeconds(1);
			m_Cube.Value = cubeValue;
			m_RotationSpeed.Value = rotationSpeedValue;

		}

		private void OnDisable() {
			m_Cube.OnValueChange -= Cube_OnValueChange;
			m_RotationSpeed.OnValueChange -= RotationSpeed_OnValueChange;
		}

		private void Update() {
			if (m_Cube.Value != null) {
				m_Cube.Value.Rotate(0, m_RotationSpeed.Value * Time.deltaTime, 0);
			}
		}

		#endregion


		#region Event Handlers

		private void Cube_OnValueChange(Transform previousValue) {
			Debug.Log($"Cube changed from {previousValue} to {m_Cube.Value}");
		}

		private void RotationSpeed_OnValueChange(float previousValue) {
			Debug.Log($"RotationSpeed changed from {previousValue} to {m_RotationSpeed.Value}");
		}

		#endregion


		#region Private Fields

		[Tooltip("A cube transform.")]
		[SerializeField]
		private ScriptableReferenceField<Transform> m_Cube;

		[Tooltip("The speed of the rotation.")]
		[SerializeField]
		private ScriptableValueField<float> m_RotationSpeed;

		#endregion


	}

}