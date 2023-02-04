namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[Serializable]
	public class CompositeObject {


		#region Public Properties

		public string Name {
			get => m_Name;
			set => m_Name = value;
		}

		#endregion


		#region Private Fields

		[SerializeField]
		private string m_Name;

		[SerializeField]
		private bool m_Edit;

		#endregion


	}

}