namespace CocodriloDog.Core {
	
	using System;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// An object that can search for and reference <see cref="CompositeObject"/>s from the current composite root 
	/// or chosen one.
	/// </summary>
	/// 
	/// <remarks>
	/// For this system to work, the <see cref="CompositeObject"/> instance must be registered before
	/// <see cref="Value"/> is requested, this should be done normally on <c>Awake</c>. For the 
	/// <see cref="CompositeObject"/> to be garbage collected, the instance must be unregistered, 
	/// normally <c>OnDestroy</c>. For this purpose, you can use 
	/// <see cref="CompositeObject.RegisterAsReferenceable(UnityEngine.Object)"/> and
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
					Debug.LogWarning("CompositeObjectReference<T>.Value only works at runtime.");
				}
				var compositeObject = ReferenceableCompositeObjects.GetById(m_Source, m_Id);
				if(compositeObject == null) {
					Debug.LogWarning("CompositeObject not found. Did you forget to register it with RegisterAsReferenceable()?");
				}
				return compositeObject as T;
			}
		}

		/// <summary>
		/// Shows/hides the <see cref="m_Source"/> field in the inspector.
		/// </summary>
		/// <remarks>
		/// This should be set <c>OnValidate</c>.
		/// </remarks>
		public bool ShowSource {
			get => m_ShowSource;
			set => m_ShowSource = value;
		}

		/// <summary>
		/// Enables/disables the <see cref="m_Source"/> field in the inspector.
		/// </summary>
		/// <remarks>
		/// This should be set <c>OnValidate</c>.
		/// </remarks>
		public bool EnableChooseSource {
			get => m_EnableChooseSource;
			set => m_EnableChooseSource = value;
		}

		/// <summary>
		/// Shows/hides the toggle <see cref="m_EnableChooseSource"/> in the inspector.
		/// </summary>
		/// <remarks>
		/// This should be set <c>OnValidate</c>.
		/// </remarks>
		public bool ShowEnableChooseSource {
			get => m_ShowEnableChooseSource;
			set => m_ShowEnableChooseSource = value;
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Used to set the source to <c>null</c> so that the inspector resets it to the default root object
		/// when <see cref="m_EnableChooseSource"/> is <c>false</c>.
		/// </summary>
		/// <remarks>
		/// Only has effect when not overriding the source, because otherwise the user may want to preserve 
		/// the overriding <see cref="m_Source"/>.
		/// </remarks>
		public void ClearSourceToDefault() {
			if (!m_EnableChooseSource) {
				m_Source = null;
			}
		}

		#endregion


		#region Private Fields

		[Tooltip("The root of the CompositeObject.")]
		[SerializeField]
		private UnityEngine.Object m_Source;

		[SerializeField]
		private bool m_ShowSource = true;

		[Tooltip("Allows to choose another object as the source root.")]
		[FormerlySerializedAs("m_OverrideSource")]
		[SerializeField]
		private bool m_EnableChooseSource;

		[FormerlySerializedAs("m_AllowOverrideSource")]
		[SerializeField]
		private bool m_ShowEnableChooseSource = true;

		[Tooltip("The unique Id of the CompositeObject.")]
		[SerializeField]
		private string m_Id;

		#endregion


	}

}