namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Allows to call the <see cref="Object.Destroy(Object)"/> from a Unity event.
	/// </summary>
	public class DestroyAdapter : MonoBehaviour {


		#region Public Methods

		public void DestroyInstance(Object obj) => Destroy(obj);

		#endregion


	}

}