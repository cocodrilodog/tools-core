namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class CompositeRoot : MonoBehaviour {


		#region Public Properties

		public CompositeObject SelectedCompositeObject {
			get => m_SelectedCompositeObject;
			set => m_SelectedCompositeObject = value;
		}

		#endregion


		#region Private Fields

		[HideInInspector]
		[SerializeReference]
		private CompositeObject m_SelectedCompositeObject;

		#endregion


	}

}