namespace CocodriloDog.Core {

	using UnityEngine;

	public interface IUnityObject { 

		public string name { get; set; }
	
	}

	public interface IComponent : IUnityObject {

		public GameObject gameObject { get; }

		public Transform transform { get; }

	}

}