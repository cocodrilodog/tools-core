namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Base class for property drawers of <see cref="CompositeObject"/> concrete classes.
	/// </summary>
	public abstract class CompositePropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

			// Non-edit is one-field height and can be obtained from the base definition.
			// This is called first to initialize the properties correctly.
			var nonEditHeight = base.GetPropertyHeight(property, label);

			if (Property.managedReferenceValue != null && EditProperty.boolValue) {
				// Return the height for the edit mode
				return GetEditPropertyHeight(property, label);
			} else {
				// Return the single field height
				return nonEditHeight;
			}

		}

		public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);

			label = EditorGUI.BeginProperty(position, label, property);
			if (Property.managedReferenceValue != null && EditProperty.boolValue) {
				// Draw edit mode
				DrawBreadcrums();
				OnEditGUI(position, property, label);
			} else {
				// Draw non-edit mode
				OnNonEditGUI(position, property, label);
			}
			ContextMenu();
			EditorGUI.EndProperty();

		}

		#endregion


		#region Protected Properties

		/// <summary>
		/// Implement this in subclasses to obtain a list of possible concrete classes that will appear 
		/// in a context menu when the user clicks the "Create" button.
		/// </summary>
		protected abstract List<Type> CompositeTypes { get; }

		#endregion


		#region Protected Methods

		protected override void InitializePropertiesForGetHeight() {
			base.InitializePropertiesForGetHeight();
			// It seems that the properties need to be initialized in both places for it to work correctly.
			EditProperty = Property.FindPropertyRelative("m_Edit");
			NameProperty = Property.FindPropertyRelative("m_Name");
			SelectedCompositePathProperty = Property.serializedObject.FindProperty("m_SelectedCompositePath");
		}

		protected override void InitializePropertiesForOnGUI() {
			base.InitializePropertiesForOnGUI();
			// It seems that the properties need to be initialized in both places for it to work correctly,
			// especially whe the property is an array item. (So far only m_Name is failing in arrays)
			//EditProperty = Property.FindPropertyRelative("m_Edit");
			NameProperty = Property.FindPropertyRelative("m_Name");
			//SelectedCompositePathProperty = Property.serializedObject.FindProperty("m_SelectedCompositePath");
		}

		/// <summary>
		/// Gets the height needed for the property when it is in edit mode.
		/// </summary>
		/// <param name="property">The property</param>
		/// <param name="label">The label</param>
		/// <returns>The height</returns>
		protected virtual float GetEditPropertyHeight(SerializedProperty property, GUIContent label) { 
			// One-field height for the breadcrums + the name. Subclasses should add extra space for their properties
			return base.GetPropertyHeight(property, label) + EditorGUI.GetPropertyHeight(NameProperty) + 2; 
		}

		/// <summary>
		/// The <c>OnGUI</c> for the edit mode.
		/// </summary>
		/// <param name="position">The position</param>
		/// <param name="property">The property</param>
		/// <param name="label">The label</param>
		protected virtual void OnEditGUI(Rect position, SerializedProperty property, GUIContent label) {
			// This base class only handles the name property.
			EditorGUI.PropertyField(GetNextPosition(), NameProperty);		
		}

		#endregion


		#region Private Properties

		private SerializedProperty EditProperty { get; set; }

		private SerializedProperty NameProperty { get; set; }

		private SerializedProperty SelectedCompositePathProperty { get; set; }

		#endregion


		#region Private Methods - Edit Mode

		private void DrawBreadcrums() {

			var buttonRect = GetNextPosition();

			if (SelectedCompositePathProperty != null) {
				// This button will make the inspector to go back to the root
				DrawNextButton($"◂ {Property.serializedObject.targetObject.GetType().Name}", () => {
					SelectCompositeObject(null);
				});
				// Analize path parts and create a breadcrum button for each CompositeObject in the path
				var pathParts = Property.propertyPath.Split('.');
				for (int i = 0; i < pathParts.Length; i++) {

					// Each path until the i part
					var partialPath = string.Join('.', pathParts, 0, i + 1);
					var partialProperty = Property.serializedObject.FindProperty(partialPath);

					if (partialProperty != null) {

						var isManagedReference = partialProperty.propertyType == SerializedPropertyType.ManagedReference;
						if (isManagedReference) {

							// Possible composite for each partial path
							var partialComposite = partialProperty.managedReferenceValue as CompositeObject;
							if (partialComposite == Property.managedReferenceValue) {
								// The partialComposite is the main composite object of this property
								DrawNextButton($"• {partialComposite.Name}");
							} else {
								// The partialComposite is an intermediate between the root and the main
								// composite object of this property
								DrawNextButton($"◂ {partialComposite.Name}", () => {
									SelectCompositeObject(partialProperty.propertyPath);
								});
							}

						}

					}

				}
			} else {
				DrawNextButton($" ▴ ", () => EditProperty.boolValue = false);
				DrawNextButton($"{(Property.managedReferenceValue as CompositeObject).Name}");
			}

			void DrawNextButton(string label, Action action = null) {

				// Fit the button to the text
				var content = new GUIContent(label);
				var size = GUI.skin.button.CalcSize(content);
				buttonRect.width = size.x;

				EditorGUI.BeginDisabledGroup(action == null);
				if (GUI.Button(buttonRect, content)) {
					action?.Invoke();
				}
				EditorGUI.EndDisabledGroup();

				// buttonRect += button width, for the next button
				buttonRect.x += size.x + 2;

			}

		}

		private void SelectCompositeObject(string propertyPath) {

			// Set to non-edit the previosly selected composite object
			if (!string.IsNullOrEmpty(SelectedCompositePathProperty.stringValue)) {
				var selectedCompositeObjectProperty = Property.serializedObject.FindProperty(SelectedCompositePathProperty.stringValue);
				selectedCompositeObjectProperty.FindPropertyRelative("m_Edit").boolValue = false;
			}

			// Assign the new value
			SelectedCompositePathProperty.stringValue = propertyPath;

			// Set to edit the new selected composite object
			if (!string.IsNullOrEmpty(SelectedCompositePathProperty.stringValue)) {
				var selectedCompositeObjectProperty = Property.serializedObject.FindProperty(SelectedCompositePathProperty.stringValue);
				selectedCompositeObjectProperty.FindPropertyRelative("m_Edit").boolValue = true;
			}

			// Set any selected field to be non-selected. Otherwise the text content of a selected field
			// will remain displayed, even if a new selected text field has other text.
			GUI.FocusControl(null);

		}

		#endregion


		#region Private Methods - Non Edit Mode

		private void OnNonEditGUI(Rect position, SerializedProperty property, GUIContent label) {

			// Main rect
			var mainRect = GetNextPosition();

			// Buttons rect
			var buttonsWidth = 120;
			var buttonsRect = mainRect;
			buttonsRect.xMin += mainRect.width - buttonsWidth;

			// Property rects
			var propertyRect = mainRect;
			propertyRect.xMax -= buttonsWidth + 2;
			
			// Button specific rects
			var firstButtonRect = buttonsRect;
			firstButtonRect.width *= 0.5f;

			var secondButtonRect = buttonsRect;
			secondButtonRect.xMin += buttonsRect.width * 0.5f;

			if (Property.managedReferenceValue == null) {
				// Create button
				DrawPropertyField(propertyRect, $"{Property.displayName}", $"Null");
				DrawCreateButton(firstButtonRect);
			} else {
				// Edit button
				if (CDEditorUtility.GetElementIndex(Property, out var index)) {
					DrawPropertyField(propertyRect, $"Element {index}", $"{NameProperty.stringValue}");
				} else {
					DrawPropertyField(propertyRect, $"{Property.displayName}", $"{NameProperty.stringValue}");
				}
				DrawEditButton(firstButtonRect);
			}

			// Remove button
			DrawRemoveButton(secondButtonRect);

		}

		private void DrawPropertyField(Rect propertyRect, string label, string name) {

			// Label rect
			var labelWidth = Position.width * 0.25f;
			var labelRect = propertyRect;
			labelRect.width = labelWidth;

			// Field rect
			var fieldRect = propertyRect;
			fieldRect.xMin += labelWidth + 2;

			// Create a label with the property name
			EditorGUI.LabelField(labelRect, label);

			// Create a box resembling an Object field
			EditorGUI.BeginDisabledGroup(true);
			GUI.Box(fieldRect, name, EditorStyles.objectField);
			EditorGUI.EndDisabledGroup();

		}

		private void DrawCreateButton(Rect rect) {

			if (GUI.Button(rect, "Create")) {

				// Save the path for later because it will be used by the GenericMenu which happens later
				var pendingProperty = Property.Copy(); // Copy, just in case

				// Show the menu only when there are more than one types
				if (CompositeTypes.Count > 1) {
					var menu = new GenericMenu();
					foreach (var type in CompositeTypes) {
						menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(type.Name)), false, () => {
							CreateObject(type, pendingProperty);
						});
					}
					menu.ShowAsContext();
				} else {
					// When there is only one type, create the object immediatly
					CreateObject(CompositeTypes[0], pendingProperty);
				}

			}

		}

		void CreateObject(Type t, SerializedProperty property) {

			// Delay the action, otherwise, the object won't "stick" around due to the GenericMenu timing
			EditorApplication.delayCall += () => {

				property.serializedObject.Update();

				var compositeObject = Activator.CreateInstance(t) as CompositeObject;
				compositeObject.Name = t.Name;
				property.managedReferenceValue = compositeObject;

				property.serializedObject.ApplyModifiedProperties();

				property = null;

			};
		}

		private void DrawEditButton(Rect rect) {
			if(SelectedCompositePathProperty != null) {
				if (GUI.Button(rect, "Edit ▸")) {
					SelectCompositeObject(Property.propertyPath);
				} 
			} else {
				if (GUI.Button(rect, "Edit ▾")) {					
					EditProperty.boolValue = true;
				}
			}
		}

		private void DrawRemoveButton(Rect rect) {
			EditorGUI.BeginDisabledGroup(Property.managedReferenceValue == null);
			if (GUI.Button(rect, "Remove")) {
				Property.managedReferenceValue = null;
			}
			EditorGUI.EndDisabledGroup();
		}

		private void ContextMenu() {

			Event current = Event.current;

			if (Position.Contains(current.mousePosition) && current.type == EventType.ContextClick) {

				// Save the property for later because it will be used by the GenericMenu which happens later
				var pendingProperty = Property.Copy(); // Copy, just in case

				GenericMenu menu = new GenericMenu();
				if (Property.managedReferenceValue != null) {
					menu.AddItem(new GUIContent("Copy"), false, () => {
						CompositeCopier.Copy(pendingProperty.managedReferenceValue as CompositeObject);
					});
				} else {
					menu.AddDisabledItem(new GUIContent("Copy"));
				}

				var propertyType = CDEditorUtility.GetManagedReferenceType(pendingProperty);
				if(propertyType.IsAssignableFrom(CompositeCopier.CopiedType)) {
					menu.AddItem(new GUIContent("Paste"), false, () => {
						// Delay the action, otherwise, the object won't "stick" around due to the GenericMenu timing
						EditorApplication.delayCall += () => {
							pendingProperty.serializedObject.Update();
							pendingProperty.managedReferenceValue = CompositeCopier.Paste();
							pendingProperty.serializedObject.ApplyModifiedProperties();
						};
					});
				} else {
					menu.AddDisabledItem(new GUIContent("Paste"));
				}

				menu.ShowAsContext();
				current.Use();

			}

		}

		#endregion


	}

}