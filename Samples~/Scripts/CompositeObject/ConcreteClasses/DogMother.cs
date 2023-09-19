namespace CocodriloDog.Core.Examples {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[Serializable]
	public class DogMother : Dog {


		#region Public Fields

		[SerializeReference]
		public List<Dog> Puppies;

		[SerializeReference]
		public Dog Male;

		#endregion


	}

}
