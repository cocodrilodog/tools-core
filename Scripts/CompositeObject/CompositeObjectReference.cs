namespace CocodriloDog.Core {
	
	using System;
	using UnityEngine;

	/// <summary>
	/// An object that can search for and reference <see cref="CompositeObject"/>s of the current root 
	/// </summary>
	/// <typeparam name="T">The type of <see cref="CompositeObject"/> to look for</typeparam>
	[Serializable]
	public class CompositeObjectReference<T> where T : CompositeObject {


		#region Public Properties

		/// <summary>
		/// The referenced <see cref="CompositeObject"/>.
		/// </summary>
		public T Value => m_Value;

		#endregion


		#region Private Fields

		[Tooltip("The root of the CompositeObject.")]
		[SerializeField]
		private UnityEngine.Object m_Source;

		[Tooltip("Allows to choose another object as the source root.")]
		[SerializeField]
		private bool m_OverrideSource;

		[Tooltip("The reference to the CompositeObject.")]
		[SerializeReference]
		private T m_Value;

		#endregion


	}

}