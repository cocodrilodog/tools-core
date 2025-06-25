namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Runtime storage of all the <see cref="CompositeObject"/>s by <c>Id</c> that are planned to be reachable by
	/// <see cref="CompositeObjectReference{T}"/>.
	/// </summary>
	public static class ReferenceableCompositeObjects {


		#region Public Static Methods

		/// <summary>
		/// Registers a <see cref="CompositeObject"/> for it to be reachable by <see cref="CompositeObjectReference{T}"/>.
		/// </summary>
		/// <param name="root">The root Unity.Object that contains the <see cref="CompositeObject"/></param>
		/// <param name="compositeObject">The <see cref="CompositeObject"/>.</param>
		public static void Register(UnityEngine.Object root, CompositeObject compositeObject) {
			s_ReferenceableCompositeObjects.TryAdd(root, new Dictionary<string, CompositeObject>());
			s_ReferenceableCompositeObjects[root][compositeObject.Id] = compositeObject;
			//Debug.Log($"Registered: {compositeObject.Name} {compositeObject.Id}");
		}

		/// <summary>
		/// Unregisters a <see cref="CompositeObject"/> so that it is no longer reachable by <see cref="CompositeObjectReference{T}"/>.
		/// </summary>
		/// <param name="root">The root Unity.Object that contains the <see cref="CompositeObject"/></param>
		/// <param name="compositeObject">The <see cref="CompositeObject"/>.</param>
		/// <returns></returns>
		public static bool Unregister(UnityEngine.Object root, CompositeObject compositeObject) {
			var result = false;
			if (s_ReferenceableCompositeObjects.TryGetValue(root, out var compositeObjectsById)) {
				result = compositeObjectsById.Remove(compositeObject.Id);
				if (result) {
					//Debug.Log($"Unregistered: {compositeObject.Name} {compositeObject.Id}");
					if (compositeObjectsById.Count == 0) {
						s_ReferenceableCompositeObjects.Remove(root);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Gets a registered <see cref="CompositeObject"/> by its <paramref name="root"/> and <paramref name="id"/>.
		/// </summary>
		/// <param name="root">The root Unity.Object that contains the <see cref="CompositeObject"/></param>
		/// <param name="id">The <c>Id</c></param>
		/// <returns>The <see cref="CompositeObject"/>.</returns>
		public static CompositeObject GetById(UnityEngine.Object root, string id) {
			CompositeObject compositeObject = null;
			if (s_ReferenceableCompositeObjects.TryGetValue(root, out var compositeObjectsById)) {
				compositeObjectsById.TryGetValue(id, out compositeObject);
			}
			return compositeObject;
		}

		/// <summary>
		/// Removes all registered <see cref="CompositeObject"/>s.
		/// </summary>
		public static void Purge() => s_ReferenceableCompositeObjects.Clear();

		#endregion


		#region Private Static Fields

		private static Dictionary<UnityEngine.Object, Dictionary<string, CompositeObject>> s_ReferenceableCompositeObjects = new();

		#endregion


	}

}