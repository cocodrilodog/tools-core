namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class CompositeRoot : MonoBehaviour {


		#region Public Properties

		public string SelectedCompositePath {
			get => m_SelectedCompositePath;
			set => m_SelectedCompositePath = value;
		}

		#endregion


		#region Private Fields

		//[HideInInspector]
		[SerializeField]
		private string m_SelectedCompositePath;

		#endregion


	}

}