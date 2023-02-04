namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	public abstract class CompositePropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var nonEditHeight = base.GetPropertyHeight(property, label);
			if (Property.managedReferenceValue != null && EditProperty.boolValue) {
				return GetEditPropertyHeight(property, label);
			} else {
				return nonEditHeight;
			}
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);

			label = EditorGUI.BeginProperty(position, label, property);
			if (Property.managedReferenceValue != null && EditProperty.boolValue) {
				DrawBreadcrums();
				OnEditGUI(position, property, label);
			} else {
				OnNonEditGUI(position, property, label);
			}
			ContextMenu();
			EditorGUI.EndProperty();

		}

		#endregion


		#region Protected Properties

		protected abstract List<Type> CompositeTypes { get; }

		#endregion


		#region Protected Methods

		protected override void InitializePropertiesForGetHeight() {
			base.InitializePropertiesForGetHeight();
			EditProperty = Property.FindPropertyRelative("m_Edit");
			NameProperty = Property.FindPropertyRelative("m_Name");
			SelectedCompositePathProperty = Property.serializedObject.FindProperty("m_SelectedCompositePath");
		}

		protected override void InitializePropertiesForOnGUI() {
			base.InitializePropertiesForOnGUI();
			EditProperty = Property.FindPropertyRelative("m_Edit");
			NameProperty = Property.FindPropertyRelative("m_Name");
			SelectedCompositePathProperty = Property.serializedObject.FindProperty("m_SelectedCompositePath");
		}

		protected virtual float GetEditPropertyHeight(SerializedProperty property, GUIContent label) { 
			return base.GetPropertyHeight(property, label) + EditorGUI.GetPropertyHeight(NameProperty); 
		}

		protected virtual void OnEditGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.PropertyField(GetNextPosition(), Property.FindPropertyRelative("m_Name"));		
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

			DrawButton($"◂ {Property.serializedObject.targetObject.GetType().Name}", () => {
				SelectCompositeObject(null);
			});

			var pathParts = Property.propertyPath.Split('.');
			for (int i = 0; i < pathParts.Length; i++) {

				var partialPath = string.Join('.', pathParts, 0, i + 1);
				var partialProperty = Property.serializedObject.FindProperty(partialPath);

				if (partialProperty != null) {

					var isManagedReference = partialProperty.propertyType == SerializedPropertyType.ManagedReference;
					if (isManagedReference) {

						var tempComposite = partialProperty.managedReferenceValue as CompositeObject;
						if (tempComposite == Property.managedReferenceValue) {
							DrawButton($"• {tempComposite.Name}");
						} else {
							DrawButton($"◂ {tempComposite.Name}", () => {
								SelectCompositeObject(partialProperty.propertyPath);
							});
						}

					}

				}

			}

			void DrawButton(string label, Action action = null) {

				var content = new GUIContent(label);
				var size = GUI.skin.button.CalcSize(content);
				buttonRect.width = size.x;

				EditorGUI.BeginDisabledGroup(action == null);
				if (GUI.Button(buttonRect, content)) {
					action?.Invoke();
				}
				EditorGUI.EndDisabledGroup();

				buttonRect.x += size.x;

			}

		}

		private void SelectCompositeObject(string propertyPath) {

			if (!string.IsNullOrEmpty(SelectedCompositePathProperty.stringValue)) {
				var selectedCompositeObjectProperty = Property.serializedObject.FindProperty(SelectedCompositePathProperty.stringValue);
				selectedCompositeObjectProperty.FindPropertyRelative("m_Edit").boolValue = false;
			}

			SelectedCompositePathProperty.stringValue = propertyPath;

			if (!string.IsNullOrEmpty(SelectedCompositePathProperty.stringValue)) {
				var selectedCompositeObjectProperty = Property.serializedObject.FindProperty(SelectedCompositePathProperty.stringValue);
				selectedCompositeObjectProperty.FindPropertyRelative("m_Edit").boolValue = true;
			}

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

			var labelWidth = EditorGUIUtility.labelWidth * 0.6f;
			var labelRect = propertyRect;
			labelRect.width += labelWidth;

			var fieldRect = propertyRect;
			fieldRect.xMin += labelWidth + 2;

			EditorGUI.LabelField(labelRect, label);
			EditorGUI.BeginDisabledGroup(true);
			GUI.Box(fieldRect, name, EditorStyles.objectField);
			EditorGUI.EndDisabledGroup();

		}

		private void DrawCreateButton(Rect rect) {

			if (GUI.Button(rect, "Create")) {

				// Save the path for later because it will be used by the GenericMenu which happens later
				var pendingPropertyPath = Property.propertyPath;

				// Show the menu only when there are more than one types
				if (CompositeTypes.Count > 1) {
					var menu = new GenericMenu();
					foreach (var type in CompositeTypes) {
						menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(type.Name)), false, () => {
							CreateObject(type, pendingPropertyPath);
						});
					}
					menu.ShowAsContext();
				} else {
					CreateObject(CompositeTypes[0], pendingPropertyPath);
				}

			}

			void CreateObject(Type t, string propertyPath) {

				// Delay the action, otherwise, the object won't "stick" around due to the GenericMenu timing
				EditorApplication.delayCall += () => {

					Property.serializedObject.Update();
					var compositeObject = Activator.CreateInstance(t) as CompositeObject;
					compositeObject.Name = t.Name;

					Property.serializedObject.FindProperty($"{propertyPath}").managedReferenceValue = compositeObject;
					propertyPath = null;

					Property.serializedObject.ApplyModifiedProperties();

				};
			}

		}

		private void DrawEditButton(Rect rect) {
			if (GUI.Button(rect, "Edit ▸")) {
				SelectCompositeObject(Property.propertyPath);
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

				// Save the path for later because it will be used by the GenericMenu which happens later
				var pendingContextPath = Property.propertyPath;

				GenericMenu menu = new GenericMenu();
				if (Property.managedReferenceValue != null) {
					menu.AddItem(new GUIContent("Copy"), false, () => {
						var pendingProperty = Property.serializedObject.FindProperty(pendingContextPath);
						CompositeCopier.Copy(pendingProperty.managedReferenceValue as CompositeObject);
					});
				} else {
					menu.AddDisabledItem(new GUIContent("Copy"));
				}

				menu.AddItem(new GUIContent("Paste"), false, () => {
					// Delay the action, otherwise, the object won't "stick" around due to the GenericMenu timing
					EditorApplication.delayCall += () => {
						var pendingProperty = Property.serializedObject.FindProperty(pendingContextPath);
						pendingProperty.serializedObject.Update();
						pendingProperty.managedReferenceValue = CompositeCopier.Paste();
						pendingProperty.serializedObject.ApplyModifiedProperties();
					};
				});

				menu.ShowAsContext();
				current.Use();

			}

		}

		#endregion


	}

}