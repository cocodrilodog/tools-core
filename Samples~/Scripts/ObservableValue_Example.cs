namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using TMPro;
	using UnityEngine;

	public class ObservableValue_Example : MonoBehaviour {


		#region Unity Methods

		private void OnEnable() => m_DataModel.Score.OnValueChange += Score_OnValueChange;

		private void Start() => m_DataModel.Score.Value = 0;

		private void OnDisable() => m_DataModel.Score.OnValueChange -= Score_OnValueChange;

		#endregion


		#region Event Handlers

		public void PlusButton_onClick() => m_DataModel.Score.Value++;

		public void MinusButton_onClick() => m_DataModel.Score.Value--;

		private void Score_OnValueChange(int previousValue) {
			foreach(var text in m_Texts) {
				text.text = m_DataModel.Score.Value.ToString();
			}
		}

		#endregion


		#region Private Fields

		[SerializeField]
		private DataModel_Example m_DataModel;

		[SerializeField]
		private List<TMP_Text> m_Texts;

		#endregion


	}

}