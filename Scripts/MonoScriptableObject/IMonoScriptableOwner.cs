namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// This interface should be implemented by any component or asset that owns <see cref="MonoScriptableObject"/>
	/// properties. If implemented properly, it will fix the duplication of references to the same <see cref="MonoScriptableObject"/>s.
	/// </summary>
	/// 
	/// <remarks>
	/// This happens when the owner game object is duplicated, when an owner component is copied/pasted, when
	/// a MonoScriptableObject array size is increased and the items are duplicated and when a <see cref="MonoScriptableField{T}"/>
	/// is copied/pasted.
	/// </remarks>
	public interface IMonoScriptableOwner {


		#region Methods

		/// <summary>
		/// This will be invoked when a new component that implements this <see cref="IMonoScriptableOwner"/> is duplicated,
		/// either by duplicating a game object or by copying/pasting the component. Any existing reference to a 
		/// <see cref="MonoScriptableObject"/> instance in the original game object/component will be duplicated and will 
		/// point to the same original <see cref="MonoScriptableObject"/> instance owned by the original game object/component. 
		/// 
		/// Hence the implementation of the method should make sure that all the <see cref="MonoScriptableObject"/> instances
		/// owned by the new game object/component or its children are reinstantiated and independent from the original 
		/// game object/component.
		/// </summary>
		/// 
		/// <remarks>
		/// If this is implemented by child assets too, a chain of invocation should start on the game object and continue until
		/// the deepest <see cref="IMonoScriptableOwner"/> property. For example, a MonoBehaviour that has a <see cref="MonoScriptableObject"/>
		/// property and implements <see cref="IMonoScriptableOwner"/> should reinstantiate the <see cref="MonoScriptableObject"/>
		/// and if that asset has <see cref="MonoScriptableObject"/> properties and implements <see cref="IMonoScriptableOwner"/> too, 
		/// it should reinstantiate its own <see cref="MonoScriptableObject"/> instances as well.
		/// 
		/// Implementations will usually use:
		/// <see cref="MonoScriptableUtility.RecreateMonoScriptableObject{T}(MonoScriptableField{T}, Object)"/> and
		/// <see cref="MonoScriptableUtility.RecreateMonoScriptableObjects{T}(MonoScriptableField{T}[], Object)"/>
		/// </remarks>
		void OnMonoScriptableOwnerCreated();

		/// <summary> 
		/// This will be invoked when a component or asset that implements <see cref="IMonoScriptableOwner"/>
		/// has changes in their properties in edit mode. When an array or list of <see cref="MonoScriptableObject"/>
		/// increases in size, the newly added elements are duplicates of the last item. This may be the case given that the
		/// property changes of the asset or component may be the result of an array size increase.
		/// </summary>
		/// 
		/// <remarks>
		/// At this time, the implementation of this method should search for duplicate references of 
		/// <see cref="MonoScriptableObject"/> in arrays or lists and reinstantiate them.
		/// 
		/// Implementations will usually use:
		/// <see cref="MonoScriptableUtility.RecreateRepeatedMonoScriptableArrayOrListItems{T}(MonoScriptableField{T}[], Object)"/> and
		/// </remarks>
		void OnMonoScriptableOwnerModified();

		/// <summary>
		/// This will be invoked when a ContextMenu operation has taken place on a <see cref="IMonoScriptableOwner"/>
		/// Through the <paramref name="propertyPath"/>, it specifies the property involved with the operation so that 
		/// the related <see cref="MonoScriptableObject"/> can be recreated by an implementation.
		/// </summary>
		/// 
		/// <remarks>
		/// This was created so that when a <see cref="MonoScriptableObject"/> is copied/pasted, the pasted version is
		/// recreated and now is independent from the original one. Unfortunately, this will cause any right-clicked field 
		/// (even the one that is copied) to recreate their corresponding <see cref="MonoScriptableObject"/>, which is 
		/// not needed. This can be improved if I find a way to handle when a value is pasted.
		/// 
		/// Implementations will usually use:
		/// <see cref="MonoScriptableUtility.RecreateMonoScriptableObjectAtPath{T}(string, Object)"/> and
		/// </remarks>
		/// 
		/// <param name="propertyPath">The propertyPath, as defined by SerializedObject.</param>
		void OnMonoScriptableOwnerContextMenu(string propertyPath);

		#endregion


	}

}
