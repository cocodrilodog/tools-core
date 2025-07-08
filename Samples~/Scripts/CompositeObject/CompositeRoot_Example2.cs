namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// This shows how to use a <see cref="CompositeObject"/> in a <see cref="MonoCompositeRoot"/>
	/// derivative class. Here, navigation through the composite hierarchy is easier to carry out
	/// with the support of the breadcrums, and sibling menus.
	/// </summary>
	[AddComponentMenu("")]
	public class CompositeRoot_Example2 : MonoCompositeRoot {


		#region Unity Methods

		private void OnEnable() {
			// Make it available for CompositeObjectReference<T>
			m_MyDisk.RegisterAsReferenceable(this);
		}

		private void OnDisable() {
			// Clean up
			m_MyDisk.UnregisterReferenceable(this);
		}

		#endregion


		#region Private Fields

		[SerializeReference]
		private Folder m_MyDisk;

		#endregion


	}

}