namespace CocodriloDog.Core {

	using UnityEngine;
	using UnityEditor;

	/// <summary>
	/// This adds copy/paste functionality to <see cref="CompositeObject"/>.
	/// </summary>
	[InitializeOnLoad]
	public static class CompositeContextMenuHandler {


		#region Static Constructors

		static CompositeContextMenuHandler() {
			EditorApplication.contextualPropertyMenu += OnContextMenuOpening;
		}

		#endregion


		#region Event Handlers

		private static void OnContextMenuOpening(GenericMenu menu, SerializedProperty property) {
			if (property.propertyType == SerializedPropertyType.ManagedReference && // <- The property is a managed reference
				typeof(CompositeObject).IsAssignableFrom(CDEditorUtility.GetPropertyType(property))) { // <- The property is a CompositeObject
				ContextMenu(menu, property);
			}
		}

		#endregion


		#region Private Static Methods

		private static void ContextMenu(GenericMenu menu, SerializedProperty property) {

			var pendingProperty = property.Copy(); // Copy, just in case
			var propertyType = CDEditorUtility.GetPropertyType(pendingProperty);
			var copyName = "";

			if (property.managedReferenceValue != null) {
				copyName = ObjectNames.NicifyVariableName((property.managedReferenceValue as CompositeObject).Name);
				menu.AddItem(new GUIContent($"Copy {copyName}"), false, () => {
					CompositeCopier.Copy(pendingProperty.managedReferenceValue as CompositeObject);
				});
			} else {
				copyName = ObjectNames.NicifyVariableName(propertyType.Name);
				menu.AddDisabledItem(new GUIContent($"Copy  {copyName}"));
			}

			var pasteName = CompositeCopier.CopiedObject != null ? ObjectNames.NicifyVariableName(CompositeCopier.CopiedObject.Name) : "";

			// The copied concrete CompositeObject is of the property type
			if (propertyType.IsAssignableFrom(CompositeCopier.CopiedType)) {
				menu.AddItem(new GUIContent($"Paste {pasteName}"), false, () => {
					// Delay the action, otherwise, the object won't "stick" around due to the GenericMenu timing
					EditorApplication.delayCall += () => {
						pendingProperty.serializedObject.Update();
						pendingProperty.managedReferenceValue = CompositeCopier.Paste();
						pendingProperty.serializedObject.ApplyModifiedProperties();
					};
				});
			} else {
				menu.AddDisabledItem(new GUIContent($"Paste {pasteName}"));
			}

			menu.ShowAsContext();

		}

		#endregion


	}

}