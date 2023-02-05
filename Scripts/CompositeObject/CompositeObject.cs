namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A base <see cref="object"/> class that can be extended to create composite structures
	/// on top of a <see cref="CompositeRoot"/> which is s <see cref="MonoBehaviour"/>.
	/// </summary>
	/// 
	/// <remarks>
	/// <see cref="CompositeRoot"/> can have child <see cref="CompositeObject"/>s and <see cref="CompositeObject"/>s
	/// can also have child <see cref="CompositeObject"/>s, hence the composite name.
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
	///			<term>Extend <see cref="CompositeRoot"/></term>
	///			<description>
	///				Create a concrete extension of <see cref="CompositeRoot"/> that will be the 
	///				root or first parent of the concrete <see cref="CompositeObject"/> instances. 
	///			</description>
	///		</item>
	///		<item>
	///			<term>Extend <c>CompositeRootEditor</c></term>
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

		/// <summary>
		/// The name of the object
		/// </summary>
		public string Name {
			get => m_Name;
			set => m_Name = value;
		}

		#endregion


		#region Private Fields

		[SerializeField]
		private string m_Name;

#if UNITY_EDITOR
		[SerializeField]
		private bool m_Edit;
#endif

		#endregion


	}

}