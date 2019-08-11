namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class BaseClass_Example : MonoBehaviour {

		// The headers in this example are a generic way of grouping the class members
		// but feel free to order them differently, as long as it makes sense and helps
		// to understand their nature.

		#region Public Fields

		// Notice that the order of the attributes is inverted
		[Header("References")]
		[Space]

		// When dividing a script with the HorizontalLine use CAPS on the enclosing
		// header. That way, if there are subheaders, they can use normal title format
		// and be differentiated from the main headers.
		[Header("BASE CLASS")]
		[HorizontalLine]

		[SerializeField]
		public GameObject SomeReference;

		[SerializeField]
		public Renderer SomeOtherReference;

		[Header("Values")]

		[SerializeField]
		public float SomeValue;

		[SerializeField]
		public string SomeOtherValue;

		#endregion


		#region Private Fields - Serialized

		[Header("Subcomponents")]

		[SerializeField]
		private Transform m_SomeSubcomponent;

		[SerializeField]
		private Rigidbody m_SomeOtherSubcomponent;

		#endregion


	}

}