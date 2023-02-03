namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class Composite_Example : CompositeRoot {


		#region Public Fields

		[SerializeReference]
		public Dog Composite;

		[SerializeReference]
		public List<Dog> Composites;

		#endregion


	}

}