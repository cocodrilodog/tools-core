namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A base <see cref="System.Object"/> class that can be extended to create composite and polymorphic 
	/// structures on top of a MonoBehaviour, ScriptableObject, or preferably <see cref="MonoCompositeRoot"/>
	/// and <see cref="ScriptableCompositeRoot"/>
	/// </summary>
	/// 
	/// <remarks>
	/// 
	/// <para>
	/// <see cref="CompositeObject"/>s can have <see cref="CompositeObject"/> children, hence the name
	/// "Composite". They can exist in MonoBehaviour and ScriptableObject, but the inspector is more usable if 
	/// they are implemented in a <see cref="MonoCompositeRoot"/> or <see cref="ScriptableCompositeRoot"/>.
	/// </para>
	/// 
	/// <para>
	/// For single instances of <see cref="CompositeObject"/>, they must be declared using the <see cref="SerializeReference"/>
	/// attribute. Lists and arrays would also require the <see cref="SerializeReference"/> attribute, but it is strongly
	/// recommended to use <see cref="CompositeList{T}"/> with the normal <see cref="SerializeField"/>, instead. 
	/// <see cref="CompositeList{T}"/> handles a list internally with <see cref="SerializeReference"/> and handles 
	/// prefab use cases that causes prefab corruption, due to a Unity bug.
	/// </para>
	/// 
	/// <para>
	/// If the <see cref="CompositeObject"/> is intended to be referenced by a <see cref="CompositeObjectReference{T}"/>
	/// you must invoke <see cref="RegisterAsReferenceable(UnityEngine.Object)"/> on Awake and 
	/// <see cref="UnregisterReferenceable(UnityEngine.Object)"/> on destroy.
	/// </para>
	/// 
	/// </remarks>
	[Serializable]
	public abstract class CompositeObject {


		#region Public Properties


		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name {
			get => m_Name;
			set => m_Name = value;
		}

		/// <summary>
		/// A unique Id for this object.
		/// </summary>
		public string Id => m_Id;

		/// <summary>
		/// Enables or disables the edit button in the inspector.
		/// </summary>
		/// <remarks>
		/// This should be called from <c>OnValidate</c>
		/// </remarks>
		public bool CanEnterEdit {
			get => m_CanEnterEdit;
			set => m_CanEnterEdit = value;
		}

		/// <summary>
		/// Enables or disables the name field in the inspector.
		/// </summary>
		/// <remarks>
		/// This should be called from <c>OnValidate</c>
		/// </remarks>
		public bool CanEditName {
			get => m_CanEditName;
			set => m_CanEditName = value;
		}

		/// <summary>
		/// Enables or disables the delete button in the inspector.
		/// </summary>
		/// <remarks>
		/// This should be called from <c>OnValidate</c>
		/// </remarks>
		public bool CanDeleteInstance {
			get => m_CanDeleteInstance;
			set => m_CanDeleteInstance = value;
		}

#if UNITY_EDITOR
		/// <summary>
		/// This flag is used by the editor tools to show the <see cref="CompositeObject"/> exapanded
		/// (<c>Edit = true</c>) or contracted as a one line field (<c>Edit = false</c>)
		/// </summary>
		public bool Edit {
			get => m_Edit;
			set => m_Edit = value;
		}

		/// <summary>
		/// In the inspector, shows the <see cref="m_DocumentationComment"/> property field when is 
		/// <c>true</c>.
		/// </summary>
		public bool EditDocumentationComment {
			get => m_EditDocumentationComment;
			set => m_EditDocumentationComment = value;
		}
#endif

		/// <summary>
		/// Override this if you want to change the name that is displayed in the fields
		/// and siblings menu.
		/// </summary>
		public virtual string DisplayName => Name;

		/// <summary>
		/// Override this if you want to change the default name of the object.
		/// </summary>
		public virtual string DefaultName => GetType().Name;

		#endregion


		#region Public Constructor

		public CompositeObject() => m_Name = DefaultName;

		#endregion


		#region Public Methods

		/// <summary>
		/// Registers this <see cref="CompositeObject"/> so that it is reachable by a <see cref="CompositeObjectReference{T}"/>
		/// at runtime. Normally this would be called on <c>Awake</c>.
		/// </summary>
		/// <remarks>
		/// Don't forget to call <see cref="UnregisterReferenceable(UnityEngine.Object)"/>.
		/// </remarks>
		/// <param name="root">The root object where the <see cref="CompositeObject"/> belongs.</param>
		public virtual void RegisterAsReferenceable(UnityEngine.Object root) => ReferenceableCompositeObjects.Register(root, this);

		/// <summary>
		/// Unregisters this <see cref="CompositeObject"/>. Normally this would be called <c>OnDestroy</c>. 
		/// </summary>
		/// <remarks>
		/// Important: When the object was previosuly registered, not calling this may result in the object not being garbage collected. 
		/// </remarks>
		/// <param name="root">The root object where the <see cref="CompositeObject"/> belongs.</param>
		public virtual void UnregisterReferenceable(UnityEngine.Object root) => ReferenceableCompositeObjects.Unregister(root, this);

		#endregion


		#region Private Fields

		[Tooltip("The name of this CompositeObject")]
		[SerializeField]
		private string m_Name;

		[HideInInspector]
		[SerializeField]
		private string m_Id = Guid.NewGuid().ToString();

		[HideInInspector]
		[SerializeField]
		private bool m_CanEnterEdit = true;

		[HideInInspector]
		[SerializeField]
		private bool m_CanEditName = true;

		[HideInInspector]
		[SerializeField]
		private bool m_CanDeleteInstance = true;

#if UNITY_EDITOR
		[NonSerialized]
		private bool m_Edit;

		[TextArea]
		[SerializeField]
		private string m_DocumentationComment;

		[NonSerialized]
		private bool m_EditDocumentationComment;
#endif

		#endregion


	}

}