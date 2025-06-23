namespace CocodriloDog.Core {

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
		public static void Register(CompositeObject compositeObject) => s_RuntimeObjects[compositeObject.Id] = compositeObject;

		/// <summary>
		/// Unregisters a <see cref="CompositeObject"/>.
		/// </summary>
		/// <param name="compositeObject">The <see cref="CompositeObject"/>.</param>
		/// <returns></returns>
		public static bool Unregister(CompositeObject compositeObject) => s_RuntimeObjects.ContainsKey(compositeObject.Id);

		/// <summary>
		/// Gets a registered <see cref="CompositeObject"/> by its <c>Id</c>.
		/// </summary>
		/// <param name="id">The <c>Id</c></param>
		/// <returns>The <see cref="CompositeObject"/>.</returns>
		public static CompositeObject GetRuntimeCompositeObjectById(string id) {
			// Force initialization of MonoUpdater if it haven't been initialized. This will ensure the OnAwakeEv
			// to take place. This event will trigger all composite objects to register on the CompositeObjectsRuntime
			// so that it can be found here.
			_ = MonoUpdater.Instance;
			return s_RuntimeObjects[id];
		}

		#endregion


		#region Private Static Fields

		private static Dictionary<string, CompositeObject> s_RuntimeObjects = new();

		#endregion


	}

}