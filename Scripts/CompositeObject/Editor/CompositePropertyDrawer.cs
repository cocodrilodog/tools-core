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

			if (Property.managedReferenceValue != null && EditProperty.boolValue) {
				DrawBreadcrums();
				OnEditGUI(position, property, label);
			} else {

				// Main rect
				var mainRect = GetNextPosition();

				// Buttons rect
				var buttonsWidth = 120;
				var buttonsRect = mainRect;
				buttonsRect.xMin += mainRect.width - buttonsWidth;

				// Specific rects
				var labelRect = mainRect;
				labelRect.xMax -= buttonsWidth;

				var firstButtonRect = buttonsRect;
				firstButtonRect.width *= 0.5f;

				var secondButtonRect = buttonsRect;
				secondButtonRect.xMin += buttonsRect.width * 0.5f;

				if (Property.managedReferenceValue == null) {
					// Create button
					EditorGUI.LabelField(labelRect, "[Null]");
					DrawCreateButton(firstButtonRect);
				} else {
					// Edit button
					EditorGUI.LabelField(labelRect, NameProperty.stringValue);
					DrawEditButton(firstButtonRect);
				}

				// Remove button
				DrawRemoveButton(secondButtonRect);

			}

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

		//private CompositeRoot Root => Property.serializedObject.targetObject as CompositeRoot;

		private SerializedProperty EditProperty { get; set; }

		private SerializedProperty NameProperty { get; set; }

		private SerializedProperty SelectedCompositePathProperty { get; set; }

		#endregion


		#region Private Methods

		private void DrawCreateButton(Rect rect) {

			// Save the path because when the object is an array element, it will need the path with the index
			// in order to assign the value to the correct slot in the deferred assignment below. Otherwise it 
			// will always assign the value to the first slot.
			var pendingPropertyPath = Property.propertyPath;

			if (GUI.Button(rect, "Create")) {
				// Show the menu only when there are more than one types
				if (CompositeTypes.Count > 1) {
					var menu = new GenericMenu();
					foreach (var type in CompositeTypes) {
						menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(type.Name)), false, () => {
							CreateObject(type);
						});
					}
					menu.ShowAsContext();
				} else {
					CreateObject(CompositeTypes[0]);
				}
			}

			void CreateObject(Type t) {

				// Delay the action, otherwise, the object won't "stick" around
				CDEditorUtility.DelayedAction(() => {

					Property.serializedObject.Update();
					var compositeObject = Activator.CreateInstance(t) as CompositeObject;
					compositeObject.Name = t.Name;

					Property.serializedObject.FindProperty($"{pendingPropertyPath}").managedReferenceValue = compositeObject;
					pendingPropertyPath = null;

					Property.serializedObject.ApplyModifiedProperties();

				}, 0);
			}

		}

		private void DrawEditButton(Rect rect) {
			if (GUI.Button(rect, "Edit ▸")) {
				//Property.FindPropertyRelative("m_Edit").boolValue = true;
				//SelectedCompositePathProperty.stringValue = Property.propertyPath;
				SetSelectedCompositePath(Property.propertyPath);
				Debug.Log(SelectedCompositePathProperty.stringValue);
			}
		}

		private void DrawRemoveButton(Rect rect) {
			EditorGUI.BeginDisabledGroup(Property.managedReferenceValue == null);
			if (GUI.Button(rect, "Remove")) {
				Property.managedReferenceValue = null;
			}
			EditorGUI.EndDisabledGroup();
		}

		private void DrawBreadcrums() {

			var buttonRect = GetNextPosition();
			//var closeFollowing = false;

			DrawButton($"◂ {Property.serializedObject.targetObject.GetType().Name}", () => {
				//SelectedCompositePathProperty.stringValue = null;
				SetSelectedCompositePath(null);
				Debug.Log(SelectedCompositePathProperty.stringValue);
				//closeFollowing = true;
			});

			var pathParts = Property.propertyPath.Split('.');
			for (int i = 0; i < pathParts.Length; i++) {

				var partialPath = string.Join('.', pathParts, 0, i + 1);
				var partialProperty = Property.serializedObject.FindProperty(partialPath);

				if (partialProperty != null) {

					var isManagedReference = partialProperty.type.Contains("managedReference");
					if (isManagedReference) {

						var tempComposite = partialProperty.managedReferenceValue as CompositeObject;
						//tempComposite.Edit = !closeFollowing;
						if (tempComposite == Property.managedReferenceValue) {
							DrawButton($"• {tempComposite.Name}");
						} else {
							DrawButton($"◂ {tempComposite.Name}", () => {
								//SelectedCompositePathProperty.stringValue = partialProperty.propertyPath;
								SetSelectedCompositePath(partialProperty.propertyPath);
								Debug.Log(SelectedCompositePathProperty.stringValue);
								//closeFollowing = true;
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

		private void SetSelectedCompositePath(string path) {
			if (!string.IsNullOrEmpty(SelectedCompositePathProperty.stringValue)) {
				var selectedCompositeObjectProperty = Property.serializedObject.FindProperty(SelectedCompositePathProperty.stringValue);
				selectedCompositeObjectProperty.FindPropertyRelative("m_Edit").boolValue = false;
			}
			SelectedCompositePathProperty.stringValue = path;
			if (!string.IsNullOrEmpty(SelectedCompositePathProperty.stringValue)) {
				var selectedCompositeObjectProperty = Property.serializedObject.FindProperty(SelectedCompositePathProperty.stringValue);
				selectedCompositeObjectProperty.FindPropertyRelative("m_Edit").boolValue = true;
			}
		}

		#endregion


	}

}