namespace CocodriloDog.Core {

	using System;
	using System.Collections;
    using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

    /// <summary>
    /// A base class to create fields in MonoBehaviours with references to 
    /// <see cref="MonoScriptableObject"/>
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type of the <see cref="MonoScriptableObject"/> to be referenced
    /// </typeparam>
    public abstract class MonoScriptableField<T> : MonoScriptableFieldBase where T : MonoScriptableObject {


		#region Public Properties

		/// <summary>
		/// The concrete type reference to <see cref="MonoScriptableObject"/>
		/// </summary>
		public T Object => m_Object;

		/// <summary>
		/// The base type reference to <see cref="MonoScriptableObject"/>
		/// </summary>
		public override MonoScriptableObject ObjectBase => Object;

		#endregion


		#region Public Methods

		/// <summary>
		/// Sets the concrete <see cref="MonoScriptableObject"/>.
		/// </summary>
		/// <param name="value">The object</param>
		public void SetObject(T value) => m_Object = value;

		public override IMonoScriptableOwner Recreate(GameObject gameObject) {
			if (Object != null) {

				Debug.Log($"Recreating: {Object.ObjectName}");

				// Create a new component
				var clone = Undo.AddComponent(gameObject, Object.GetType());

				// Paste the values of the current component in the new one
				UnityEditorInternal.ComponentUtility.CopyComponent(Object);
				UnityEditorInternal.ComponentUtility.PasteComponentValues(clone);

				// Replace the old component
				SetObject(clone as T);
				clone.hideFlags = HideFlags.HideInInspector;

				return clone as IMonoScriptableOwner;

			} else {
				return null;
			}
		}

		#endregion


		#region Private Fields

		[SerializeField]
        private T m_Object;

		#endregion


	}

	/// <summary>
	/// Base class to be used as a common interface to access fields of any concrete types.
	/// </summary>
	public abstract class MonoScriptableFieldBase {

		/// <summary>
		/// The <see cref="MonoScriptableObject"/> of this field in its base type.
		/// </summary>
		public abstract MonoScriptableObject ObjectBase { get; }

		/// <summary>
		/// Recreates the <see cref="Object"/>
		/// </summary>
		/// <param name="gameObject">
		/// The <c>GameObject</c> to which the <see cref="MonoScriptableObject"/> clone will 
		/// be attached to.
		/// </param>
		/// <returns>
		/// The clone, if it is a <see cref="IMonoScriptableOwner"/>, otherwise <c>null</c>
		/// </returns>
		public abstract IMonoScriptableOwner Recreate(GameObject gameObject);

	}

}