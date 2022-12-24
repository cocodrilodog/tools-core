namespace CocodriloDog.Core {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

	/// <summary>
	/// Base class for <c>ScriptableObjects</c> that will be created on top of <c>MonoBehaviours</c> 
	/// with the help of <see cref="MonoScriptableField{T}"/>s.
	/// </summary>
	public class MonoScriptableObject : ScriptableObject {


		#region Public Methods

		/// <summary>
		/// Sets the owner of this MonoScriptableObject.
		/// </summary>
		/// <remarks>
		/// This will be handled automatically by the related Unity editor tools.
		/// </remarks>
		/// <param name="owner">The owner of this MonoScriptableObject</param>
		public void SetOwner(Object owner) {
			m_Owner = owner;
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// The owner of this object. It can be a <c>MonoBehaviour</c> or other MonoScriptableObject
		/// </summary>
		public Object Owner => m_Owner;

		#endregion


		#region Private Fields

		[SerializeField]
		private Object m_Owner;

		#endregion


	}

}