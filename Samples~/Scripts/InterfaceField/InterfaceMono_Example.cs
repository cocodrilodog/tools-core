namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class InterfaceMono_Example : MonoBehaviour, IAnyInterface {


		#region Public Properties

		public string SomeProperty => m_SomeProperty;

		#endregion


		#region Public Methods

		public void SomeMethod() => Debug.Log(m_SomeMethodText);

		#endregion


		#region Private Fields

		[SerializeField]
		private string m_SomeProperty = "This is SomeProperty";

		[SerializeField]
		private string m_SomeMethodText = "This is SomeMethod";

		#endregion


	}

}