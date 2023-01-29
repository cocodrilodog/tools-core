namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// This interface should be implemented by any component that owns <see cref="MonoCompositeObject"/>
	/// properties. If implemented properly, it will fix the duplication of references to the same 
	/// <see cref="MonoCompositeObject"/>s and the leftover <see cref="MonoCompositeObject"/> components.
	/// </summary>
	/// 
	/// <remarks>
	/// The duplicate references to the same <see cref="MonoCompositeObject"/> happen when 
	/// the <see cref="MonoCompositeRoot"/> is copied/pasted, when a <see cref="MonoCompositeObject"/> 
	/// array size is modified and when a <see cref="MonoCompositeField{T}"/> is copied/pasted.
	/// 
	/// The leftover <see cref="MonoCompositeObject"/> happens when it is no longer owned by
	/// any other object so it needs to be removed from the <c>GameObject</c>.
	/// </remarks>
	public interface IMonoCompositeParent {


		#region Methods

		/// <summary>
		/// This method should return an array of all the <see cref="MonoCompositeFieldBase"/> that are 
		/// owned by <c>this</c>.
		/// </summary>
		/// <returns></returns>
		MonoCompositeFieldBase[] GetChildren();

		/// <summary>
		/// This method should call <see cref="MonoCompositeObject.SetParent(MonoBehaviour)"/> on all the 
		/// owned <see cref="MonoCompositeObject"/>s and pass <c>this</c> as parameter.
		/// </summary>
		void ConfirmChildren();

		#endregion


	}

}
