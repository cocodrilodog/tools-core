namespace CocodriloDog.Core.Examples {

	using CocodriloDog.Core;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A folder that can contain both folders and <see cref="TextFile"/>s
	/// </summary>
	[Serializable]
	public class Folder : FileBase {


		#region Public Methods

		public override void RegisterAsReferenceable(UnityEngine.Object root) {
			base.RegisterAsReferenceable(root);
			m_Files.ForEach(f => f.RegisterAsReferenceable(root));
		}

		public override void UnregisterReferenceable(UnityEngine.Object root) {
			base.UnregisterReferenceable(root);
			m_Files.ForEach(f => f.UnregisterReferenceable(root));
		}

		#endregion


		#region Private Fields

		/// <summary>
		/// For lists of <see cref="CompositeObject"/>s, it is recommended to use a subclass of 
		/// CompositeList&lt;&gt; which has a custom property drawer that prevents serialized data 
		/// corruption on the [SerializeReference] objects that are stored in a prefab.
		/// </summary>
		[SerializeField]
		private CompositeList<FileBase> m_Files;

		#endregion


	}

}