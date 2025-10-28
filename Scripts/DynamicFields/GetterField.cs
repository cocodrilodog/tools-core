namespace CocodriloDog.Core {

	using System;
	using UnityEngine;

	[Serializable]
	public class GetterField<T> {


		#region Public Properties

		public UnityEngine.Object Object => m_Object;

		#endregion


		#region Private Fields

		[SerializeField]
		private UnityEngine.Object m_Object;

		[SerializeField]
		private string m_Path;

		#endregion


	}

}