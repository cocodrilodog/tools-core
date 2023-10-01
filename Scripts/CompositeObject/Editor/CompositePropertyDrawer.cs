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

			if (CanEdit) {
				// Return the height for the edit mode
				return Edit_GetPropertyHeight(property, label);
			} else {
				// Return the single field height
				return nonEditHeight;
			}

		}

		public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);

			label = EditorGUI.BeginProperty(position, label, property);
			if (CanEdit) {
				// Draw edit mode
				DrawBreadcrums();
				Edit_OnGUI(position, property, label);
			} else {
				// Draw non-edit mode
				NonEdit_OnGUI(position, property, label);
			}
			EditorGUI.EndProperty();

		}

		#endregion


		#region Protected Properties

		/// <summary>
		/// Implement this in subclasses to obtain a list of possible concrete classes that will appear 
		/// in a context menu when the user clicks the "Create" button.
		/// </summary>
		protected abstract List<Type> CompositeTypes { get; }

		protected bool CanEdit => Property.managedReferenceValue != null && CompositeObject.Edit;

		#endregion


		#region Protected Methods

		protected sealed override void InitializePropertiesForGetHeight() {
			
			base.InitializePropertiesForGetHeight();
			
			//EditProperty = Property.FindPropertyRelative("m_Edit");
			NameProperty = Property.FindPropertyRelative("m_Name");
			SelectedCompositePathProperty = Property.serializedObject.FindProperty("m_SelectedCompositePath");
			
			if (CanEdit) {
				Edit_InitializePropertiesForGetHeight();
			}

		}

		protected virtual void Edit_InitializePropertiesForGetHeight() { }

		protected sealed override void InitializePropertiesForOnGUI() {
			
			base.InitializePropertiesForOnGUI();

			// It seems that the properties need to be initialized (retrieved) in both places for it to work
			// correctly, especially when the property is an array item.
			//
			// So far these have failed:
			// - m_Name
			// - m_Edit
			// - m_SelectedCompositePath

			//EditProperty = Property.FindPropertyRelative("m_Edit");
			NameProperty = Property.FindPropertyRelative("m_Name");
			SelectedCompositePathProperty = Property.serializedObject.FindProperty("m_SelectedCompositePath");

			if (CanEdit) {
				Edit_InitializePropertiesForOnGUI();
			}

		}

		protected virtual void Edit_InitializePropertiesForOnGUI() { }

		/// <summary>
		/// Gets the height needed for the property when it is in edit mode.
		/// </summary>
		/// <param name="property">The property</param>
		/// <param name="label">The label</param>
		/// <returns>The height</returns>
		protected virtual float Edit_GetPropertyHeight(SerializedProperty property, GUIContent label) {
			if (SelectedCompositePathProperty != null) {
				// There is a root, breadcrums is handled by the root.
				//
				// One-field height for the the name. 
				// Subclasses should add extra space for their properties
				return base.GetPropertyHeight(property, label); 
			} else {
				// There is no root, breadcrums are drawn here.
				//
				// One-field height for the breadcrums + the name.
				// Subclasses should add extra space for their properties
				return base.GetPropertyHeight(property, label) + EditorGUI.GetPropertyHeight(NameProperty) + 2;
			}
		}

		/// <summary>
		/// The <c>OnGUI</c> for the edit mode.
		/// </summary>
		/// <param name="position">The position</param>
		/// <param name="property">The property</param>
		/// <param name="label">The label</param>
		protected virtual void Edit_OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			// This base class only handles the name property.
			EditorGUI.PropertyField(GetNextPosition(), NameProperty);		
		}

		protected virtual void DrawPropertyField(Rect propertyRect, string label, string name) {

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

		#endregion


		#region Private Properties

		private CompositeObject CompositeObject => Property.managedReferenceValue as CompositeObject;

		private SerializedProperty NameProperty { get; set; }

		private SerializedProperty SelectedCompositePathProperty { get; set; }

		#endregion


		#region Private Methods

		private void DrawBreadcrums() {

			if (SelectedCompositePathProperty == null) {

				var buttonRect = GetNextPosition();
				DrawNextButton($" ▴ ", () => CompositeObject.Edit = false);
				DrawNextButton($"{(Property.managedReferenceValue as CompositeObject).Name}");
				
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
		}

		private void NonEdit_OnGUI(Rect position, SerializedProperty property, GUIContent label) {

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
					DrawPropertyField(propertyRect, $"Element {index}", $"{DisplayName()}");
				} else {
					DrawPropertyField(propertyRect, $"{Property.displayName}", $"{DisplayName()}");
				}
				DrawEditButton(firstButtonRect);
			}

			// Remove button
			DrawRemoveButton(secondButtonRect);

			string DisplayName() {
				return (Property.managedReferenceValue as CompositeObject).DisplayName;
			}

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
				compositeObject.Name = compositeObject.DefaultName;
				property.managedReferenceValue = compositeObject;

				property.serializedObject.ApplyModifiedProperties();

				property = null;

			};
		}

		private void DrawEditButton(Rect rect) {
			if(SelectedCompositePathProperty != null) {
				if (GUI.Button(rect, "Edit ▸")) {
					CompositeRootEditor.SelectCompositeObject(
						Property.serializedObject, SelectedCompositePathProperty, Property.propertyPath
					);
				} 
			} else {
				if (GUI.Button(rect, "Edit ▾")) {					
					CompositeObject.Edit = true;
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

		#endregion


	}

}