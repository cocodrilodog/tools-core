namespace CocodriloDog.Core {

	using UnityEngine;
	using UnityEditor;

	/// <summary>
	/// This works in conjunction with <see cref="MonoScriptableFieldPropertyDrawer"/> to enable the
	/// fields while a context menu operation is taking place. For example, for copying / pasting.
	/// </summary>
	[InitializeOnLoad]
	public static class MonoScriptableContextMenuHandler {


		#region Public Static Fields

		/// <summary>
		/// A path built by <see cref="MonoScriptableFieldPropertyDrawer"/> to identify a field
		/// that must be enabled while right clicking it. This will enable the "Copy" menu item.
		/// </summary>
		public static string EnabledFieldPath;

		#endregion


		#region Static Constructors

		static MonoScriptableContextMenuHandler() {
			EditorApplication.contextualPropertyMenu += OnContextMenuOpening;
		}

		#endregion


		#region Event Handlers

		private static void OnContextMenuOpening(GenericMenu menu, SerializedProperty property) {
			// This will be invoked after the copy or paste command have taken effect, so if the field is nulled
			// the MonoScriptableField will be disabled again.
			var monoScriptableOwner = property.serializedObject.targetObject as IMonoScriptableOwner;
			if (monoScriptableOwner != null) {
				EnabledFieldPath = null;
			}
		}

		#endregion


	}

}