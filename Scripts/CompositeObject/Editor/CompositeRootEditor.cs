namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Base class for editors of concrete implementations of <see cref="CompositeRoot"/>.
	/// </summary>
	public abstract class CompositeRootEditor : Editor {


		#region Public Static Methods

		public static void SelectCompositeObject(
			SerializedObject serializedObject,
			SerializedProperty selectedCompositePathProperty,
			string newPropertyPath
			) {

			// Set to non-edit the previosly selected composite object
			if (!string.IsNullOrEmpty(selectedCompositePathProperty.stringValue)) {
				var selectedCompositeObjectProperty = serializedObject.FindProperty(selectedCompositePathProperty.stringValue);
				selectedCompositeObjectProperty.FindPropertyRelative("m_Edit").boolValue = false;
			}

			// Assign the new value
			selectedCompositePathProperty.stringValue = newPropertyPath;

			// Set to edit the new selected composite object
			if (!string.IsNullOrEmpty(selectedCompositePathProperty.stringValue)) {
				var selectedCompositeObjectProperty = serializedObject.FindProperty(selectedCompositePathProperty.stringValue);
				selectedCompositeObjectProperty.FindPropertyRelative("m_Edit").boolValue = true;
			}

			// Set any selected field to be non-selected. Otherwise the text content of a selected field
			// will remain displayed, even if a new selected text field has other text.
			GUI.FocusControl(null);

		}

		#endregion


		#region Unity Methods

		protected virtual void OnEnable() {
			ScriptProperty = serializedObject.FindProperty("m_Script");
			SelectedCompositePathProperty = serializedObject.FindProperty("m_SelectedCompositePath");
		}

		public sealed override void OnInspectorGUI() {
			if (string.IsNullOrEmpty(SelectedCompositePathProperty.stringValue)) {
				// There is no selected composite object, proceed with the inspector of the root object
				OnRootInspectorGUI();
			} else {
				// There is a selected composite object, draw only it as a property.
				serializedObject.Update();
				CDEditorUtility.DrawDisabledField(ScriptProperty);
				var selectedCompositeProperty = serializedObject.FindProperty(SelectedCompositePathProperty.stringValue);
				if (selectedCompositeProperty != null) {
					DrawBreadcrums(selectedCompositeProperty);
					EditorGUILayout.PropertyField(selectedCompositeProperty);
				}
				serializedObject.ApplyModifiedProperties();
			}
		}

		#endregion


		#region Protected Methods

		/// <summary>
		/// In subclasses, override this instead of <see cref="OnInspectorGUI"/>.
		/// </summary>
		/// <remarks>
		/// This editor will choose to display this GUI when no <see cref="CompositeObject"/> is selected.
		/// Otherwise it will render the property drawer of the selected <see cref="CompositeObject"/>.
		/// </remarks>
		protected virtual void OnRootInspectorGUI() => base.OnInspectorGUI();

		#endregion


		#region Private Properties

		private SerializedProperty ScriptProperty { get; set; }

		private SerializedProperty SelectedCompositePathProperty { get; set; }

		#endregion


		#region Private Methods

		private void DrawBreadcrums(SerializedProperty property) {

			//var buttonRect = GetNextPosition();
			GUILayout.BeginHorizontal();
			if (SelectedCompositePathProperty != null) {
				// This button will make the inspector to go back to the root
				DrawNextButton($"◂ {target.GetType().Name}", () => {
					SelectCompositeObject(serializedObject, SelectedCompositePathProperty, null);
				});
				// Analize path parts and create a breadcrum button for each CompositeObject in the path
				var pathParts = property.propertyPath.Split('.');
				for (int i = 0; i < pathParts.Length; i++) {

					// Each path until the i part
					var partialPath = string.Join('.', pathParts, 0, i + 1);
					var partialProperty = property.serializedObject.FindProperty(partialPath);

					if (partialProperty != null) {

						var isManagedReference = partialProperty.propertyType == SerializedPropertyType.ManagedReference;
						if (isManagedReference) {

							// Possible composite for each partial path
							var partialComposite = partialProperty.managedReferenceValue as CompositeObject;
							if (partialComposite == property.managedReferenceValue) {
								// The partialComposite is the main composite object of this property
								DrawNextButton($"• {partialComposite.Name}");
							} else {
								// The partialComposite is an intermediate between the root and the main
								// composite object of this property
								DrawNextButton($"◂ {partialComposite.Name}", () => {
									SelectCompositeObject(serializedObject, SelectedCompositePathProperty, partialProperty.propertyPath);
								});
							}

						}

					}

				}
			}
			GUILayout.EndHorizontal();

			void DrawNextButton(string label, Action action = null) {

				EditorGUI.BeginDisabledGroup(action == null);
				if (GUILayout.Button(label, GUILayout.ExpandWidth(false))) {
					action?.Invoke();
				}

				// Reduce the space between the buttons
				GUILayout.Space(-3);

				EditorGUI.EndDisabledGroup();

			}

		}

		#endregion


	}

}