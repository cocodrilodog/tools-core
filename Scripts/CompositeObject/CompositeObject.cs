namespace CocodriloDog.Core {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

	/// <summary>
	/// Base class for <c>MonoBehaviour</c>s that are related to each other in a composite 
	/// structure.
	/// </summary>
	public class CompositeObject {


		#region Public Properties

		/// <summary>
		/// The name of the object, different from the <c>MonoBehaviour.name</c>.
		/// </summary>
		public string Name {
			get => m_Name;
			set => m_Name = value;
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Sets the owner of this <see cref="CompositeObject"/>.
		/// </summary>
		/// <param name="parent">The parent of this MonoCompositeObject</param>
		public void SetParent(ICompositeParent parent) {
			m_Parent = parent;
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// The owner of this object. It can be a <c>MonoBehaviour</c>, other <see cref="CompositeObject"/>
		/// or a <see cref="CompositeRoot"/>.
		/// </summary>
		public ICompositeParent Parent => m_Parent;

		#endregion


		#region Private Fields

		[SerializeField]
		private ICompositeParent m_Parent;

		[SerializeField]
		private string m_Name;

		#endregion


	}

}