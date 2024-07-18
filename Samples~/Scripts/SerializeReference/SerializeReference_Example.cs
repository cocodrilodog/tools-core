namespace CocodriloDog.Core.Examples {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class SerializeReference_Example : MonoBehaviour {

		[SerializeReference]
		public Cat CatA = new Cat();

		[SerializeReference]
		public Cat[] CatB;

	}

	[Serializable]
	public class Cat {

		[SerializeField]
		public string Name;

		public Cat() { }

		public Cat(string name) {
			Name = name;
		}

	}

	[Serializable]
	public class CatMother : Cat {

		[SerializeReference]
		public List<Cat> Kittens;

		public CatMother() { }

		public CatMother(string name) : base(name) { }

	}

}