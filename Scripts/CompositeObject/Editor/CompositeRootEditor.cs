namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
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

			GUILayout.BeginHorizontal();
			if (SelectedCompositePathProperty != null) {

				// This button will make the inspector to go back to the root
				DrawNextButton($"◂ {target.GetType().Name}", () => {
					SelectCompositeObject(serializedObject, SelectedCompositePathProperty, null);
				});

				// Analize path parts and create a breadcrum button for each CompositeObject in the path
				var pathParts = property.propertyPath.Split('.');
				SerializedProperty parentProperty = null;
				for (int i = 0; i < pathParts.Length; i++) {

					// Each path until the i part
					var partialPath = string.Join('.', pathParts, 0, i + 1);
					var partialProperty = property.serializedObject.FindProperty(partialPath);
					
					if (partialProperty != null) {

						if (IsCompositeProperty(partialProperty, out var partialComposite)) {

							// Composite for this partial path
							if (partialComposite == property.managedReferenceValue) {
								// The partialComposite is the main composite object of this property
								DrawNextButton($"• {partialComposite.Name}", parentProperty);

							} else {
								// The partialComposite is an intermediate between the root and the main
								// composite object of this property
								DrawNextButton($"◂ {partialComposite.Name}", parentProperty, () => {
									SelectCompositeObject(serializedObject, SelectedCompositePathProperty, partialProperty.propertyPath);
								});
							}

							parentProperty = partialProperty.Copy();

						}

					}

				}

			}
			GUILayout.EndHorizontal();

		}

		private bool IsCompositeProperty(SerializedProperty property, out CompositeObject compositeObject) {
			var isComposite = property.propertyType == SerializedPropertyType.ManagedReference &&
				property.managedReferenceValue is CompositeObject;
			compositeObject = null;
			if (isComposite) {
				compositeObject = property.managedReferenceValue as CompositeObject;
			}
			return isComposite;
		}

		/// <summary>
		/// Draws a breadcrums button.
		/// </summary>
		/// <param name="label">The label of the button</param>
		/// <param name="action">The action that the button will perform</param>
		private void DrawNextButton(string label, Action action = null) {
			EditorGUI.BeginDisabledGroup(action == null);
			if (GUILayout.Button(label, GUILayout.ExpandWidth(false))) {
				action?.Invoke();
			}
			// Reduce the space between the buttons
			GUILayout.Space(-3);
			EditorGUI.EndDisabledGroup();
		}

		/// <summary>
		/// Draws a breadcrums button with a <see cref="GenericMenu"/> that shows the sibling <see cref="CompositeObject"/>s
		/// of the <see cref="CompositeObject"/> of the button.
		/// </summary>
		/// <param name="label">The label of the button</param>
		/// <param name="parentProperty">
		/// The parent property to iterate over its children and display them in a <see cref="GenericMenu"/>.
		/// They are the siblings of the button's object.
		/// </param>
		/// <param name="action">The action that the button will perform</param>
		private void DrawNextButton(string label, SerializedProperty parentProperty, Action action = null) {

			// Draw the button
			EditorGUI.BeginDisabledGroup(action == null);
			if (GUILayout.Button(label + "      ", GUILayout.ExpandWidth(false))) {
				action?.Invoke();
			}

			// Create the siblings mouse area
			var padding = 3;
			var lastRect = GUILayoutUtility.GetLastRect();
			lastRect.xMin += lastRect.width - 20;
			lastRect.xMax -= padding;
			lastRect.yMin += padding;
			lastRect.yMax -= padding;
			GUI.Box(lastRect, new GUIContent(""), EditorStyles.objectField);

			// Create the siblings menu
			if (lastRect.Contains(Event.current.mousePosition)) {

				var menu = new GenericMenu();

				if (parentProperty != null) {
					// Parent property is CompositeObject
					CDEditorUtility.IterateChildProperties(parentProperty, ChildPropertyAction);
				} else {
					// Parent property is CompositeRoot
					CDEditorUtility.IterateChildProperties(serializedObject, ChildPropertyAction);
				}

				// This will receive the siblings of the button's object
				void ChildPropertyAction(SerializedProperty siblingProperty) {

					if (IsCompositeProperty(siblingProperty, out var compositeSibling)) {
						// Save for later use by the menu
						var pendingChildProperty = siblingProperty.Copy();
						var on = pendingChildProperty.propertyPath == SelectedCompositePathProperty.stringValue;
						menu.AddItem(new GUIContent(compositeSibling.Name), on, () => SelectSibling(pendingChildProperty));
					} else if (siblingProperty.isArray && siblingProperty.propertyType == SerializedPropertyType.Generic) {
						// The property is an array or list
						for (int i = 0; i < siblingProperty.arraySize; i++) {
							// Get the elements
							var element = siblingProperty.GetArrayElementAtIndex(i);
							if (element.propertyType == SerializedPropertyType.ManagedReference) {
								// It is managed reference
								compositeSibling = element.managedReferenceValue as CompositeObject;
								if (compositeSibling != null) {
									// It is CompositeObject, save for later use by the menu
									var pendingElement = element.Copy();
									var on = pendingElement.propertyPath == SelectedCompositePathProperty.stringValue;
									menu.AddItem(new GUIContent(compositeSibling.Name), on, () => SelectSibling(pendingElement));
								} else {
									// These are managed references, but not CompositeObjects 
									break;
								}
							} else {
								// This is an array of non-managed reference objects
								break;
							}
						}
					}

					// Delay the selection to overcome strange menu timing
					void SelectSibling(SerializedProperty prop) {
						EditorApplication.delayCall += () => {
							serializedObject.Update();
							SelectCompositeObject(serializedObject, SelectedCompositePathProperty, prop.propertyPath);
							serializedObject.ApplyModifiedProperties();
						};
					}

				}

				// Show the menu
				menu.ShowAsContext();

			}

			// Reduce the space between the buttons
			GUILayout.Space(-3);
			EditorGUI.EndDisabledGroup();

		}

		#endregion


	}

}