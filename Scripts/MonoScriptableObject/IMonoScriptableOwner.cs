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
	/// This happens when the owner game object is duplicated, when an owner component is copied/pasted and when
	/// a MonoScriptableObject array size is increased and the items are duplicated.
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
		/// </remarks>
		void OnMonoScriptableOwnerCreated();

		/// <summary> 
		/// This will be invoked when a component or asset that implements <see cref="IMonoScriptableOwner"/>
		/// has changes in their properties in edit mode. When an array or list of <see cref="MonoScriptableObject"/>
		/// increases in size, the newly added elements are duplicates of the last item. This may be the case given that the
		/// property changes of the asset or component may be the result of an array size increase.
		/// 
		/// At this time, the implementation of this method should search for duplicate references of 
		/// <see cref="MonoScriptableObject"/> in arrays or lists and reinstantiate them.
		/// </summary>
		void OnMonoScriptableOwnerModified();

		// TODO: Optimize implementations so that only the relevant property is recreated.
		void OnMonoScriptableOwnerContextMenu(string propertyPath);

		#endregion


	}

}
