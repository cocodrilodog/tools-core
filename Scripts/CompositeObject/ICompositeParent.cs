namespace CocodriloDog.Core {

	using CocodriloDog.Core;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public interface ICompositeParent<T> where T : CompositeObject {

		/// <summary>
		/// Gets the child <see cref="CompositeObject"/> named <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The <see cref="CompositeObject.Name"/></param>
		/// <returns>The <see cref="CompositeObject"/></returns>
		T GetChild(string name);

		/// <summary>
		/// Finds a child <see cref="CompositeObject"/> at the specified path.
		/// </summary>
		/// <param name="path">The path of the object. For example "Parallel/Sequence1/Motion2D"</param>
		/// <returns>The <see cref="CompositeObject"/> if it was found</returns>
		T GetChildAtPath(string path);

		/// <summary>
		/// Gets all the children <see cref="CompositeObject"/>.
		/// </summary>
		/// <returns>An array with the children.</returns>
		T[] GetChildren();

	}

}