namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// This interface should be implemented by any component that owns <see cref="CompositeObject"/>
	/// properties. If implemented properly, it will fix the duplication of references to the same 
	/// <see cref="CompositeObject"/>s and the leftover <see cref="CompositeObject"/> components.
	/// </summary>
	/// 
	/// <remarks>
	/// The duplicate references to the same <see cref="CompositeObject"/> happen when 
	/// the <see cref="CompositeRoot"/> is copied/pasted, when a <see cref="CompositeObject"/> 
	/// array size is modified and when a <see cref="CompositeField{T}"/> is copied/pasted.
	/// 
	/// The leftover <see cref="CompositeObject"/> happens when it is no longer owned by
	/// any other object so it needs to be removed from the <c>GameObject</c>.
	/// </remarks>
	public interface ICompositeParent {


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
