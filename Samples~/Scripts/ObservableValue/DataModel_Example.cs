namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(menuName = "Cocodrilo Dog/Core/Examples/Data Model Example")]
	public class DataModel_Example : ScriptableObject {


		#region Public Properties

		public ObservableValue<int> Score => m_Score;

		#endregion


		#region Private Fields

		[Tooltip("The score")]
		[SerializeField]
		private ObservableValue<int> m_Score;

		#endregion


	}

}