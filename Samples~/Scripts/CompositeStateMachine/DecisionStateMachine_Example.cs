namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using UnityEngine;

	[AddComponentMenu("")]
	public class DecisionStateMachine_Example : MonoBehaviour {


		#region Public Methods

		public void Stop() {
			m_IsPlaying = false;
			transform.localRotation = Quaternion.identity;
		}

		public void Play() {
			m_IsPlaying = true;
			StartCoroutine(DoPlay());
		}

		public void Pause() {
			m_IsPlaying = false;
		}

		#endregion


		#region Private Fields

		[NonSerialized]
		private bool m_IsPlaying;

		#endregion


		#region Private Methods

		private IEnumerator DoPlay() {
			while (m_IsPlaying) {
				transform.Rotate(0, 0, Time.deltaTime * -30);
				yield return null;
			}
		}

		#endregion


	}

}