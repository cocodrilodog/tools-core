namespace CocodriloDog.Core {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

	/// <summary>
	/// Base class for <c>MonoBehaviour</c>s that are related to each other in a composite 
	/// structure.
	/// </summary>
	public class MonoCompositeObject : MonoBehaviour {


		#region Public Properties

		/// <summary>
		/// The name of the object, different from the <c>MonoBehaviour.name</c>.
		/// </summary>
		public string ObjectName {
			get => m_ObjectName;
			set => m_ObjectName = value;
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Sets the owner of this <see cref="MonoCompositeObject"/>.
		/// </summary>
		/// <param name="parent">The parent of this MonoCompositeObject</param>
		public void SetParent(MonoBehaviour parent) {
			m_Parent = parent;
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// The owner of this object. It can be a <c>MonoBehaviour</c>, other <see cref="MonoCompositeObject"/>
		/// or a <see cref="MonoCompositeRoot"/>.
		/// </summary>
		public MonoBehaviour Parent => m_Parent;

		#endregion


		#region Private Fields

		[SerializeField]
		private MonoBehaviour m_Parent;

		[SerializeField]
		private string m_ObjectName;

		#endregion


	}

}