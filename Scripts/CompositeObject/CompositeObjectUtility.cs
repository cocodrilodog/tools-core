namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using UnityEngine;

	public class CompositeObjectUtility : MonoBehaviour {


		#region Public Static Methods

		/// <summary>
		/// Finds a child <typeparamref name="T"/> at the specified path.
		/// </summary>
		/// <param name="parent">The <see cref="ICompositeParent"/> where the search starts</param>
		/// <param name="path">The path of the object. For example "Parallel/Sequence1/Motion2D"</param>
		/// <returns>The <typeparamref name="T"/> if it was found</returns>
		public static T GetChildAtPath<T>(ICompositeParent<T> parent, string path) where T : CompositeObject {
			var pathParts = path.Split('/');
			T block = null;
			for (int i = 0; i < pathParts.Length; i++) {
				block = parent.GetChild(pathParts[i]);
				if (block is ICompositeParent<T>) {
					parent = (block as ICompositeParent<T>);
				} else {
					break;
				}
			}
			return block;
		}

		/// <summary>
		/// Helper method to change the <see cref="CompositeObject.Id"/> when an exteme edge case
		/// requires it.
		/// </summary>
		/// 
		/// <remarks>
		/// Only use this if you know what you are doing.
		/// </remarks>
		/// 
		/// <param name="compositeObject">The composite object.</param>
		/// <param name="newId">The new Id</param>
		public static void ChangeId(CompositeObject compositeObject, string newId) => compositeObject.Id = newId;

		#endregion


	}

}