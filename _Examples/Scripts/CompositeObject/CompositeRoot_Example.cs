namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

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