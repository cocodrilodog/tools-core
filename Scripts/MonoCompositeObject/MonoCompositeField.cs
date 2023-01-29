namespace CocodriloDog.Core {

	using System;
	using System.Collections;
    using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

    /// <summary>
    /// A base class to create fields in MonoBehaviours with references to 
    /// <see cref="MonoCompositeObject"/>
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type of the <see cref="MonoCompositeObject"/> to be referenced
    /// </typeparam>
    public abstract class MonoCompositeField<T> : MonoCompositeFieldBase where T : MonoCompositeObject {


		#region Public Properties

		/// <summary>
		/// The concrete type reference to <see cref="MonoCompositeObject"/>
		/// </summary>
		public T Object => m_Object;

		/// <summary>
		/// The base type reference to <see cref="MonoCompositeObject"/>
		/// </summary>
		public override MonoCompositeObject ObjectBase => Object;

		#endregion


		#region Public Methods

		/// <summary>
		/// Sets the concrete <see cref="MonoCompositeObject"/>.
		/// </summary>
		/// <param name="value">The object</param>
		public void SetObject(T value) => m_Object = value;

		public override IMonoCompositeParent Recreate(GameObject gameObject) {
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

				return clone as IMonoCompositeParent;

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
	public abstract class MonoCompositeFieldBase {

		/// <summary>
		/// The <see cref="MonoCompositeObject"/> of this field in its base type.
		/// </summary>
		public abstract MonoCompositeObject ObjectBase { get; }

		/// <summary>
		/// Recreates the <see cref="Object"/>
		/// </summary>
		/// <param name="gameObject">
		/// The <c>GameObject</c> to which the <see cref="MonoCompositeObject"/> clone will 
		/// be attached to.
		/// </param>
		/// <returns>
		/// The clone, if it is a <see cref="IMonoCompositeParent"/>, otherwise <c>null</c>
		/// </returns>
		public abstract IMonoCompositeParent Recreate(GameObject gameObject);

	}

}