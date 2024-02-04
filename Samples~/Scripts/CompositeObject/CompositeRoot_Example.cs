namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// This example shows a <c>MonoBehaviour</c> that has a custom <c>CompositeRootEditor</c>.
	/// If a <see cref="CompositeObject"/> is selected, its property drawer will takeover the 
	/// entire inspector and will allow to navigate from the root object to deeper levels and 
	/// vice versa via breadcrums.
	/// </summary>
	[AddComponentMenu("")]
	public class CompositeRoot_Example : CompositeRoot {


		#region Public Fields

		[Header("Composite Example (With Root)")]

		[SerializeField]
		public string OtherProperty = "Other Property";

		[SerializeField]
		public string OtherProperty2 = "Other Property2";

		[SerializeReference]
		public Dog SingleDog;

		[SerializeReference]
		public List<Dog> DogsList;

		#endregion


	}

}