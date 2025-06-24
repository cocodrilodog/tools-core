namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// This shows how to use a <see cref="CompositeObject"/> in a normal MonoBehaviour
	/// Here, navigation through the composite hierarchy is similar to Unity's default 
	/// inspector layout, not user friendly for complex data structures.
	/// </summary>
	[AddComponentMenu("")]
	public class Composite_Example : MonoBehaviour {


		#region Unity Methods

		private void OnDestroy() {
			m_MyDrive?.Dispose();
		}

		#endregion


		#region Private Fields

		[SerializeReference]
		private Folder m_MyDrive;

		#endregion


	}

}