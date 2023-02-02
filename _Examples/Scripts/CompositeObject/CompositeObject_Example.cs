namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class CompositeObject_Example : MonoBehaviour {

		[SerializeReference]
		public Dog Composite;

		[SerializeReference]
		public List<Dog> Composites;

	}

}