namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public abstract class ScriptableFieldBase {


		#region Public Properties

		public bool UseAsset {
			get => m_UseAsset;
			set => m_UseAsset = value;
		}

		#endregion


		#region Private Fields

		[SerializeField]
		private bool m_UseAsset;

		#endregion


	}

}
