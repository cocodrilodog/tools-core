namespace CocodriloDog.Core {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

	/// <summary>
	/// Base class for <c>MonoBehaviour</c>s that are related to each other in a composite 
	/// structure.
	/// </summary>
	public class MonoScriptableObject : MonoBehaviour {


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
		/// Sets the owner of this <see cref="MonoScriptableObject"/>.
		/// </summary>
		/// <param name="owner">The owner of this MonoScriptableObject</param>
		public void SetOwner(MonoBehaviour owner) {
			m_Owner = owner;
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// The owner of this object. It can be a <c>MonoBehaviour</c>, other <see cref="MonoScriptableObject"/>
		/// or a <see cref="MonoScriptableRoot"/>.
		/// </summary>
		public MonoBehaviour Owner => m_Owner;

		#endregion


		#region Private Fields

		[SerializeField]
		private MonoBehaviour m_Owner;

		[SerializeField]
		private string m_ObjectName;

		#endregion


	}

}