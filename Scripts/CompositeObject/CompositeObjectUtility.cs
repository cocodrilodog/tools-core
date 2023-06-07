namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
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

		#endregion


	}

}