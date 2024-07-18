namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	[AddComponentMenu("")]
	public class StatusLabel_Example : MonoBehaviour {


		#region Event Handlers

		public void SetStopped() {
			m_StatusLabel.color = Color.white;
			m_StatusLabel.GetComponentInChildren<TMP_Text>().text = "Stopped";
		}

		public void SetPlaying() {
			m_StatusLabel.color = Color.green;
			m_StatusLabel.GetComponentInChildren<TMP_Text>().text = "Playing";
		}

		public void SetPaused() {
			m_StatusLabel.color = Color.yellow;
			m_StatusLabel.GetComponentInChildren<TMP_Text>().text = "Paused";
		}

		#endregion


		#region Private Fields

		[SerializeField]
		private Image m_StatusLabel;

		#endregion


	}

}