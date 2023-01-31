namespace CocodriloDog.Core {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class SerializeReferenceTest : MonoBehaviour {

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
	public class MotherCat : Cat {

		[SerializeReference]
		public List<Cat> Kittens;

		public MotherCat() { }

		public MotherCat(string name) : base(name) { }

	}

}