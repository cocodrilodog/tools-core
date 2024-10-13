namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class MonoUpdater_Example : MonoBehaviour {

		private void Awake() {
			m_Updatable = new SomeUpdatableObject();
		}

		private void OnEnable() {
			MonoUpdater.AddUpdatable(m_Updatable);
			MonoUpdater.AddFixedUpdatable(m_Updatable);
		}

		private void OnDisable() {
			MonoUpdater.RemoveUpdatable(m_Updatable);
			MonoUpdater.RemoveFixedUpdatable(m_Updatable);
		}

		private SomeUpdatableObject m_Updatable;

	}

	/// <summary>
	/// An object that does not receive Update or FixeUpdate messages by default.
	/// This can be implemente in a scriptable object, for example.
	/// </summary>
	public class SomeUpdatableObject : IFixedUpdatable, IUpdatable {
		
		public void FixedUpdate() => Debug.Log("FixedUpdate");
		
		public void Update() => Debug.Log("Update");

	}

}