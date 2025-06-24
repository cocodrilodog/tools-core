namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Runtime storage of all the <see cref="CompositeObject"/>s by <c>Id</c>.
	/// </summary>
	public static class RuntimeCompositeObjects {


		#region Public Static Methods

		/// <summary>
		/// Registers a <see cref="CompositeObject"/>.
		/// </summary>
		/// <param name="compositeObject">The <see cref="CompositeObject"/>.</param>
		public static void Register(CompositeObject compositeObject) {
			// Allow overwrite, because Unity constructs the object twice:
			// 1. Edit Mode deserialization
			// 2. Scene duplication for Play Mode
			// We need the second version.
			s_RuntimeObjects[compositeObject.Id] = new WeakReference<CompositeObject>(compositeObject);
			Debug.Log($"Registered: {compositeObject.Name} {compositeObject.Id} {compositeObject.GetHashCode()}");
		}

		/// <summary>
		/// Unregisters a <see cref="CompositeObject"/>.
		/// </summary>
		/// <param name="compositeObject">The <see cref="CompositeObject"/>.</param>
		/// <returns></returns>
		public static bool Unregister(CompositeObject compositeObject) {
			var result = s_RuntimeObjects.Remove(compositeObject.Id);
			if (result) {
				Debug.Log($"Unregistered: {compositeObject.Name} {compositeObject.Id}");
			}
			return result;
		}

		/// <summary>
		/// Gets a registered <see cref="CompositeObject"/> by its <c>Id</c>.
		/// </summary>
		/// <param name="id">The <c>Id</c></param>
		/// <returns>The <see cref="CompositeObject"/>.</returns>
		public static CompositeObject GetRuntimeCompositeObjectById(string id) {
			s_RuntimeObjects[id].TryGetTarget(out var compositeObject);
			return compositeObject;
		}

		#endregion


		#region Private Static Fields

		private static Dictionary<string, WeakReference<CompositeObject>> s_RuntimeObjects = new();

		#endregion


	}

}