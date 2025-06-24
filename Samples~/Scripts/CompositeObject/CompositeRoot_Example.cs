namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// This shows how to use a <see cref="CompositeObject"/> in a <see cref="CompositeRoot"/>
	/// derivative class. Here, navigation through the composite hierarchy is easier to carry out
	/// with the support of the breadcrums, and sibling menus.
	/// </summary>
	[AddComponentMenu("")]
	public class CompositeRoot_Example : MonoCompositeRoot {


		#region Unity Methods

		private void OnDestroy() {
			m_MyDisk?.Dispose();
		}

		#endregion


		#region Private Fields

		[SerializeReference]
		private Folder m_MyDisk;

		#endregion


	}

}