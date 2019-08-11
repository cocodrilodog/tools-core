namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class DerivedClass_Example : BaseClass_Example {

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
		[Header("DERIVED CLASS")]
		[HorizontalLine]

		[SerializeField]
		public GameObject SomeReferenceDerived;

		[SerializeField]
		public Renderer SomeOtherReferenceDerived;

		[Header("Values")]

		[SerializeField]
		public float SomeValueDerived;

		[SerializeField]
		public string SomeOtherValueDerived;

		#endregion


		#region Private Fields - Serialized

		[Header("Subcomponents")]

		[SerializeField]
		private Transform m_SomeSubcomponentDerived;

		[SerializeField]
		private Rigidbody m_SomeOtherSubcomponentDerived;

		#endregion


	}

}