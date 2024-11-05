namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A base <see cref="object"/> class that can be extended to create composite and polymorphic 
	/// structures optionally (and preferrably) on top of a MonoBehaviour or optionally on top of a 
	/// <see cref="CompositeRoot"/> which is derived from <see cref="MonoBehaviour"/>.
	/// </summary>
	/// 
	/// <remarks>
	/// <see cref="CompositeObject"/>s can have children <see cref="CompositeObject"/>s hence the name
	/// "Composite". If they are implemented without a corresponding <see cref="CompositeRoot"/>,
	/// they will open and close in a similar style as serializable <see cref="object"/>s. It means that 
	/// all the <see cref="CompositeObject"/> properties their children, grand-children, etc, when opened,
	/// will be visible always one inside the other (too noisy!).
	/// 
	/// On the other hand, when a corresponding <see cref="CompositeRoot"/> is implemented, if a 
	/// <see cref="CompositeObject"/> is selected, its property drawer will takeover the entire inspector 
	/// and it will allow to navigate to its children with the "Edit" button and to its parent and sibling
	/// objects via breadcrums.
	/// 
	/// The following steps describe how to create a concrete composite system:
	/// 
	/// <list type="bullet">
	///		<item>
	///			<term>Extend <see cref="CompositeObject"/></term>
	///			<description>
	///				Create a concrete extension of <see cref="CompositeObject"/>. Any field that is referencing
	///				a instance of this class must use the <see cref="SerializeReference"/> attribute for the 
	///				system to work.
	///			</description>
	///		</item>
	///		<item>
	///			<term>Optional, but recommended: Extend <see cref="CompositeRoot"/> </term>
	///			<description>
	///				Create a concrete extension of <see cref="CompositeRoot"/> that will be the 
	///				root or first parent of the concrete <see cref="CompositeObject"/> instances.				
	///			</description>
	///		</item>
	///		<item>
	///			<term>Optional, but recommended: Use <see cref="CompositeList{T}"/> for lists.</term>
	///			<description>
	///				When using <see cref="CompositeObject"/> lists, it is better to use <see cref="CompositeList{T}"/>
	///				instead of normal lists or arrays, because <see cref="CompositeList{T}"/> handles the modification
	///				of prefabs, which prevents data corruption.
	///			</description>
	///		</item>
	///		<item>
	///			<term>Optional: Extend <c>CompositePropertyDrawer</c></term>
	///			<description>
	///				The default property drawer for composite objects allows the user to create, edit, 
	///				remove <see cref="CompositeObject"/>s and navigate through the composite structure.		
	///				Optionally, you can create a concrete extension of <c>CompositePropertyDrawer</c> and 
	///				make it a <c>CustomPropertyDrawer</c> of the concrete <see cref="CompositeObject"/> to 
	///				draw a customized version. In this case, you'll need to override 
	///				<c>CompositePropertyDrawer.UseDefaultDrawer</c> for it to return to <c>false</c>.
	///			</description>
	///		</item>
	/// </list>
	/// 
	/// </remarks>
	[Serializable]
	public abstract class CompositeObject {


		#region Public Properties


		/// <summary>
		/// The name of the object
		/// </summary>
		public string Name {
			get => m_Name;
			set => m_Name = value;
		}

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


		#region Private Fields

		[Tooltip("The name of this CompositeObject")]
		[SerializeField]
		private string m_Name;

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