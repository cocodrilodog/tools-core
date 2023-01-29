namespace CocodriloDog.Core {

	using UnityEngine;
	using UnityEditor;

	/// <summary>
	/// This works in conjunction with <see cref="MonoCompositeFieldPropertyDrawer"/> to enable the
	/// fields while a context menu operation is taking place. For example, for copying / pasting.
	/// </summary>
	[InitializeOnLoad]
	public static class MonoCompositeContextMenuHandler {


		#region Public Static Fields

		/// <summary>
		/// A path built by <see cref="MonoCompositeFieldPropertyDrawer"/> to identify a field
		/// that must be enabled while right clicking it. This will enable the "Copy" menu item.
		/// </summary>
		public static string EnabledFieldPath;

		#endregion


		#region Static Constructors

		static MonoCompositeContextMenuHandler() {
			EditorApplication.contextualPropertyMenu += OnContextMenuOpening;
		}

		#endregion


		#region Event Handlers

		private static void OnContextMenuOpening(GenericMenu menu, SerializedProperty property) {
			// This will be invoked after the copy or paste command have taken effect, so if the field is nulled
			// the MonoCompositeField will be disabled again.
			var monoCompositeParent = property.serializedObject.targetObject as IMonoCompositeParent;
			if (monoCompositeParent != null) {
				EnabledFieldPath = null;
			}
		}

		#endregion


	}

}