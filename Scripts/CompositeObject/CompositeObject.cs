namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A base <see cref="object"/> class that can be extended to create composite structures
	/// optionally on top of a <see cref="CompositeRoot"/> which is s <see cref="MonoBehaviour"/>.
	/// </summary>
	/// 
	/// <remarks>
	/// <see cref="CompositeObject"/>s can have children <see cref="CompositeObject"/>s hence the name
	/// "Composite". If they are implemented without a corresponding <see cref="CompositeRoot"/>,
	/// they will open and close in a similar style as serializable <see cref="object"/>s. It means that 
	/// all the <see cref="CompositeObject"/> properties their children, grand-children, etc, when opened,
	/// will be visible always one inside the other.
	/// 
	/// On the other hand, when a corresponding <see cref="CompositeRoot"/> and its editor is implemented,
	/// if a <see cref="CompositeObject"/> is selected, its property drawer will takeover the entire inspector 
	/// and it will allow to navigate to its children with the "Edit" button and to its parent objects via 
	/// breadcrums.
	/// 
	/// <see cref="CompositeRoot"/>s can also have child <see cref="CompositeObject"/>s, hence the 
	/// composite name.
	/// 
	/// The following steps need to be taken to create a concrete composite system:
	/// 
	/// <list type="bullet">
	///		<item>
	///			<term>Extend <see cref="CompositeObject"/></term>
	///			<description>
	///				Create a concrete extension of <see cref="CompositeObject"/>. Any field of this
	///				class must use the <see cref="SerializeReference"/> attribute.
	///			</description>
	///		</item>
	///		<item>
	///			<term>Extend <c>CompositePropertyDrawer</c></term>
	///			<description>
	///				Create a concrete extension of <c>CompositePropertyDrawer</c> and make it a 
	///				<c>CustomPropertyDrawer</c> of the concrete <see cref="CompositeObject"/> to draw 
	///				the property on top of the existing <see cref="CompositeObject"/> functionality. The existing 
	///				functionality allows the user to create, edit, remove <see cref="CompositeObject"/>s and 
	///				navigate through the composite structure.
	///			</description>
	///		</item>
	///		<item>
	///			<term>Extend <see cref="CompositeRoot"/> (Optional)</term>
	///			<description>
	///				Create a concrete extension of <see cref="CompositeRoot"/> that will be the 
	///				root or first parent of the concrete <see cref="CompositeObject"/> instances.				
	///			</description>
	///		</item>
	///		<item>
	///			<term>Extend <c>CompositeRootEditor</c> (Optional)</term>
	///			<description>
	///				Create a concrete extension of <c>CompositeRootEditor</c> and make it a <c>CustomEditor</c>
	///				of the concrete <see cref="CompositeRoot"/> so that it inherits the functionality of
	///				navigating through the composite structure.
	///			</description>
	///		</item>
	/// </list>
	/// 
	/// </remarks>
	[Serializable]
	public abstract class CompositeObject {


		#region Public Properties

#if UNITY_EDITOR
		public bool Edit {
			get => m_Edit;
			set => m_Edit = value;
		}
#endif

		/// <summary>
		/// The name of the object
		/// </summary>
		public string Name {
			get => m_Name;
			set => m_Name = value;
		}

		/// <summary>
		/// Override this if you wawnt to change the default name of the object.
		/// </summary>
		public virtual string DefaultName => GetType().Name;

		#endregion


		#region Private Fields

		[SerializeField]
		private string m_Name;

#if UNITY_EDITOR
		private bool m_Edit;
#endif

		#endregion


	}

}