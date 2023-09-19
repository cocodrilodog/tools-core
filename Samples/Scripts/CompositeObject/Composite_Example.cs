namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// This example shows this <c>MonoBehaviour</c> that has no <c>CompositeRootEditor</c>.
	/// <see cref="CompositeObject"/> fields will open and close, but the MonoBehaviour 
	/// and parent <see cref="CompositeObject"/> inspectors will remain visible all the time.
	/// </summary>
	public class Composite_Example : MonoBehaviour {


		#region Public Fields

		[Header("Composite Example (Without Root)")]

		[SerializeField]
		public string OtherProperty = "Other Property";

		[SerializeField]
		public string OtherProperty2 = "Other Property2";

		[SerializeReference]
		public Dog SingleDog;

		#endregion


	}

}