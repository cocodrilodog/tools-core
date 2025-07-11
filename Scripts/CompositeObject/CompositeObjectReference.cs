namespace CocodriloDog.Core {
	
	using System;
	using UnityEngine;

	/// <summary>
	/// An object that can search for and reference <see cref="CompositeObject"/>s of the default root 
	/// or an overriden root.
	/// </summary>
	/// 
	/// <remarks>
	/// For this system to work, the <see cref="CompositeObject"/> instance must be registered before
	/// <see cref="Value"/> is requested, normally on <c>Awake</c>. For the <see cref="CompositeObject"/>
	/// to be garbage collected, the instance must be unregistered, normally <c>OnDestroy</c>. For this purpose,
	/// you can use <see cref="CompositeObject.RegisterAsReferenceable(UnityEngine.Object)"/> and
	/// <see cref="CompositeObject.UnregisterReferenceable(UnityEngine.Object)"/>
	/// </remarks>
	/// 
	/// <typeparam name="T">The type of <see cref="CompositeObject"/> or interface to look for</typeparam>
	[Serializable]
	public class CompositeObjectReference<T> where T : class {


		#region Public Properties

		/// <summary>
		/// The referenced <see cref="CompositeObject"/>.
		/// </summary>
		public T Value {
			get {
				if (!Application.isPlaying) {
					Debug.LogWarning("CompositeObjectReference<T>.Vale only works at runtime.");
				}
				var compositeObject = ReferenceableCompositeObjects.GetById(m_Source, m_Id);
				if(compositeObject == null) {
					Debug.LogWarning("CompositeObject not found. Did you forget to register it with RegisterAsReferenceable()?");
				}
				return compositeObject as T;
			}
		}

		/// <summary>
		/// Enables/disables the option to <see cref="m_OverrideSource"/> in the inspector.
		/// </summary>
		/// <remarks>
		/// This should be set <c>OnValidate</c>.
		/// </remarks>
		public bool AllowOverrideSource {
			get => m_AllowOverrideSource;
			set => m_AllowOverrideSource = value;
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Used to set the source to <c>null</c> so that the inspector resets it to the default root object
		/// when <see cref="m_OverrideSource"/> is <c>false</c>.
		/// </summary>
		/// <remarks>
		/// Only has effect when not overriding the source, because otherwise the user may want to preserve 
		/// the overriding <see cref="m_Source"/>.
		/// </remarks>
		public void ClearSourceToDefault() {
			if (!m_OverrideSource) {
				m_Source = null;
			}
		}

		#endregion


		#region Private Fields

		[Tooltip("The root of the CompositeObject.")]
		[SerializeField]
		private UnityEngine.Object m_Source;

		[SerializeField]
		private bool m_AllowOverrideSource = true;

		[Tooltip("Allows to choose another object as the source root.")]
		[SerializeField]
		private bool m_OverrideSource;

		[Tooltip("The unique Id of the CompositeObject.")]
		[SerializeField]
		private string m_Id;

		#endregion


	}

}