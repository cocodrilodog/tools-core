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

		public bool Edit {
			get => m_Edit;
			set => m_Edit = value;
		}

		public object Parent => m_Parent;

		#endregion


		#region Public Methods

		public void SetParent(object value) => m_Parent = value;

		#endregion


		#region Private Fields

		[SerializeField]
		private string m_Name;

		[SerializeField]
		private bool m_Edit;

		[SerializeField]
		private object m_Parent;

		#endregion


	}

}