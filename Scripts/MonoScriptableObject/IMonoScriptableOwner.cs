namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// This interface should be implemented by any component that owns <see cref="MonoScriptableObject"/>
	/// properties. If implemented properly, it will fix the duplication of references to the same 
	/// <see cref="MonoScriptableObject"/>s and the leftover <see cref="MonoScriptableObject"/> components.
	/// </summary>
	/// 
	/// <remarks>
	/// The duplicate references to the same <see cref="MonoScriptableObject"/> happen when 
	/// the <see cref="MonoScriptableRoot"/> is copied/pasted, when a <see cref="MonoScriptableObject"/> 
	/// array size is modified and when a <see cref="MonoScriptableField{T}"/> is copied/pasted.
	/// 
	/// The leftover <see cref="MonoScriptableObject"/> happens when it is no longer owned by
	/// any other object so it needs to be removed from the <c>GameObject</c>.
	/// </remarks>
	public interface IMonoScriptableOwner {


		#region Methods

		/// <summary>
		/// This method should return an array of all the <see cref="MonoScriptableFieldBase"/> that are 
		/// owned by <c>this</c>.
		/// </summary>
		/// <returns></returns>
		MonoScriptableFieldBase[] GetMonoScriptableFields();

		/// <summary>
		/// This method should call <see cref="MonoScriptableObject.SetOwner(MonoBehaviour)"/> on all the 
		/// owned <see cref="MonoScriptableObject"/>s and pass <c>this</c> as parameter.
		/// </summary>
		void ConfirmOwnership();

		#endregion


	}

}
