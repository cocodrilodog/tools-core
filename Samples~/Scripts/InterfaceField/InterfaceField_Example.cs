namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class InterfaceField_Example : MonoBehaviour {


		#region Unity Methods

		private void Start() {

			Debug.Log(m_AnyInterfaceObject.Value.SomeProperty);
			m_AnyInterfaceObject.Value.SomeMethod();

			Debug.Log(m_AnyInterfaceObjects[0].Value.SomeProperty);
			m_AnyInterfaceObjects[0].Value.SomeMethod();

			Debug.Log(m_AnyInterfaceObjects[1].Value.SomeProperty);
			m_AnyInterfaceObjects[1].Value.SomeMethod();

			Debug.Log(m_AnyScriptableInterfaceObject.Value.SomeProperty);
			m_AnyScriptableInterfaceObject.Value.SomeMethod();

		}

		#endregion


		#region Private Fields

		[Tooltip("A single instance")]
		[SerializeField]
		private InterfaceField<IAnyInterface> m_AnyInterfaceObject;

		[Tooltip("A single list")]
		[SerializeField]
		private List<InterfaceField<IAnyInterface>> m_AnyInterfaceObjects;

		[Tooltip("A scriptable referencee")]
		[SerializeField]
		private ScriptableInterfaceField<IAnyInterface> m_AnyScriptableInterfaceObject;

		#endregion


	}

}