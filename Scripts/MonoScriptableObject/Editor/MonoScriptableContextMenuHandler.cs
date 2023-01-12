namespace CocodriloDog.Core {

	using UnityEngine;
	using UnityEditor;

	/// <summary>
	/// This works in conjunction with <see cref="MonoScriptableFieldPropertyDrawer"/> to enable the
	/// fields while a context menu operation is taking place. For example, for copying / pasting or 
	/// applying changes to a prefab.
	/// </summary>
	[InitializeOnLoad]
	public static class MonoScriptableContextMenuHandler {


		#region Public Static Fields

		/// <summary>
		/// A path built by <see cref="MonoScriptableFieldPropertyDrawer"/> to identify a field
		/// that must be enabled while richt clicking it.
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
			var monoScriptableOwner = property.serializedObject.targetObject as IMonoScriptableOwner;
			if (monoScriptableOwner != null) {

				EnabledFieldPath = null;
				
				// This needs a small delay for the MonoScriptableObjects to be instantiated before they are recreated
				CDEditorUtility.DelayedAction(() => {
					monoScriptableOwner.OnMonoScriptableOwnerContextMenu(property.propertyPath);
				}, 0.1f);

				Debug.Log(property.propertyPath);

			}
		}

		#endregion


	}

}