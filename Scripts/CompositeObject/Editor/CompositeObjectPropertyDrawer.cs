namespace CocodriloDog.Core {

	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Base class for property drawers of <see cref="CompositeObject"/> concrete classes.
	/// </summary>
	[CustomPropertyDrawer(typeof(CompositeObject), true)]
	public class CompositeObjectPropertyDrawer : PropertyDrawerBase {


		#region Public Static Methods

		/// <summary>
		/// Returns a cached icon if available. Otherwise loads, caches, and retruns it.
		/// </summary>
		/// <remarks>
		/// This supports caching several icons for different object types because when drawing items
		/// in a list, this drawer may be reused for the elements of the list.
		/// </remarks>
		/// <param name="objectTypeName">The object type name</param>
		/// <returns>The icon texture</returns>
		public static Texture GetObjectIcon(Type objectType) {
			var objectTypeName = objectType?.Name;
			if (!string.IsNullOrEmpty(objectTypeName)) {
				// There is a valid object type name
				if (s_ObjectIcons.ContainsKey(objectTypeName)) {
					// Is already loaded, so return it
					return s_ObjectIcons[objectTypeName];
				} else {
					// load it for the first time and return it
					var objectIcon = Resources.Load($"{objectTypeName} Icon") as Texture;
					if (objectIcon != null) {
						return s_ObjectIcons[objectTypeName] = objectIcon;
					}
				}
			}
			return null;
		}

		#endregion


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
		/// <true> when the composite object can enter edit mode.
		/// </summary>
		protected bool CanEdit => Property.managedReferenceValue != null && CompositeObject.Edit;

		/// <summary>
		/// If <true>, derived composite objects will automatically draw all of their properties.
		/// This can be overriden to <c>false</c> if you want to customize the derived property drawer.
		/// </summary>
		protected virtual bool UseDefaultDrawer => true;

		/// <summary>
		/// Reference to the target <see cref="CompositeObject"/>.
		/// </summary>
		protected CompositeObject CompositeObject => Property.managedReferenceValue as CompositeObject;

		/// <summary>
		/// The property of the <c>m_Name</c> field
		/// </summary>
		protected SerializedProperty NameProperty { get; private set; }

		/// <summary>
		/// The property of the <c>m_DocumentationComment</c> field
		/// </summary>
		protected SerializedProperty DocumentationCommentProperty { get; private set; }

		#endregion


		#region Protected Methods

		protected sealed override void InitializePropertiesForGetHeight() {
			base.InitializePropertiesForGetHeight();
			NameProperty = Property.FindPropertyRelative("m_Name");
			DocumentationCommentProperty = Property.FindPropertyRelative("m_DocumentationComment");
			if (CanEdit) {
				Edit_InitializePropertiesForGetHeight();
			}
		}

		/// <summary>
		/// Called in <see cref="InitializePropertiesForGetHeight"/> when in edit mode.
		/// </summary>
		/// <remarks>
		/// Override this for properties to be ready for the <c>GetHeight()</c> call.
		/// </remarks>
		protected virtual void Edit_InitializePropertiesForGetHeight() { }

		protected sealed override void InitializePropertiesForOnGUI() {

			base.InitializePropertiesForOnGUI();

			// It seems that the properties need to be initialized (retrieved) in both places for it to work
			// correctly, especially when the property is an array item.
			//
			// So far these have failed:
			// - m_Name

			NameProperty = Property.FindPropertyRelative("m_Name");
			DocumentationCommentProperty = Property.FindPropertyRelative("m_DocumentationComment");

			if (CanEdit) {
				Edit_InitializePropertiesForOnGUI();
			}

		}

		/// <summary>
		/// Called in <see cref="InitializePropertiesForOnGUI"/> when in edit mode.
		/// </summary>
		/// <remarks>
		/// Override this for properties to be ready for the <c>OnGUI()</c> call.
		/// </remarks>
		protected virtual void Edit_InitializePropertiesForOnGUI() { }

		/// <summary>
		/// Gets the height needed for the property when it is in edit mode.
		/// </summary>
		/// <param name="property">The property</param>
		/// <param name="label">The label</param>
		/// <returns>The height</returns>
		protected virtual float Edit_GetPropertyHeight(SerializedProperty property, GUIContent label) {

			float height = 0;

			var compositeRoot = property.serializedObject.targetObject as ICompositeRoot;

			if (compositeRoot != null) {
				// There is a root, breadcrums is handled by the root.
				//
				// One-field height for the the name. 
				// Subclasses should add extra space for their properties
				height += base.GetPropertyHeight(property, label);
			} else {
				// There is no root, breadcrums are drawn here.
				//
				// One-field height for the breadcrums + the name.
				// Subclasses should add extra space for their properties
				height += base.GetPropertyHeight(property, label) + EditorGUI.GetPropertyHeight(NameProperty) + 2;
			}

			if ((Property.managedReferenceValue as CompositeObject).EditDocumentationComment) {
				height += EditorGUI.GetPropertyHeight(DocumentationCommentProperty) + 2;
				height += EditorGUIUtility.singleLineHeight + 2;
				height += 10; // Give dome space before owner and reuse id
			}

			if (UseDefaultDrawer) {

				// Get the type to get the methods with button later
				if (m_Type == null) {
					m_Type = CDEditorUtility.GetPropertyType(Property);
				}

				// Iterate the properties
				var index = 0;
				CDEditorUtility.IterateChildProperties(Property, p => {
					if (p.propertyPath != NameProperty.propertyPath &&
						p.propertyPath != DocumentationCommentProperty.propertyPath) {

						// Add the property height
						height += GetChildPropertyHeight(p);

						// Add the height of the buttons of the methods with button
						height += MethodsWithButtonUtility.GetMethodButtonsHeightAtPropertyIndex(index, m_Type);

						index++;

					}
				});
			}

			return height;

		}

		/// <summary>
		/// Returns the height used by the child property.
		/// </summary>
		/// <remarks>
		/// Override this to modify specific property heights.
		/// </remarks>
		/// <param name="property">The property</param>
		/// <returns>The height</returns>
		protected virtual float GetChildPropertyHeight(SerializedProperty property) {
			return EditorGUI.GetPropertyHeight(property) + 2;
		}

		/// <summary>
		/// The <c>OnGUI</c> for the edit mode.
		/// </summary>
		/// <param name="position">The position</param>
		/// <param name="property">The property</param>
		/// <param name="label">The label</param>
		protected virtual void Edit_OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			// Name property.
			var rect = GetNextPosition();
			var nameRect = rect;
			nameRect.xMax -= 22;

			EditorGUI.BeginDisabledGroup(!CompositeObject.CanEditName);
			EditorGUI.PropertyField(nameRect, NameProperty);
			EditorGUI.EndDisabledGroup();

			// Documentation comment button
			var documentationRect = rect;
			documentationRect.xMin = nameRect.xMax + 2;

			var compositeObject = Property.managedReferenceValue as CompositeObject;

			GUIContent documentationGUIContent = null;
			documentationGUIContent = EditorGUIUtility.IconContent("_Help@2x");
			if (string.IsNullOrWhiteSpace(DocumentationCommentProperty.stringValue)) {
				documentationGUIContent.tooltip = "Click to add a documentation comment.";
			} else {
				documentationGUIContent.tooltip = DocumentationCommentProperty.stringValue + " (Click to edit)";
			}

			if (GUI.Button(documentationRect, documentationGUIContent, EditorStyles.iconButton)) {
				compositeObject.EditDocumentationComment = !compositeObject.EditDocumentationComment;
			}

			// Documentation comment
			if (compositeObject.EditDocumentationComment) {
				EditorGUI.PropertyField(GetNextPosition(DocumentationCommentProperty), DocumentationCommentProperty);
				if (GUI.Button(GetNextPosition(), "Done")) {
					compositeObject.EditDocumentationComment = false;
				}
				GetNextPosition(10f);
			}

			// Default drawer
			if (UseDefaultDrawer) {

				CDEditorUtility.GetPropertyValueAndType(Property, out var value, out m_Type);
				var index = 0;

				CDEditorUtility.IterateChildProperties(Property, p => {
					if (p.propertyPath != NameProperty.propertyPath &&
						p.propertyPath != DocumentationCommentProperty.propertyPath) {

						// Draw the child property
						DrawChildProperty(p);

						// Draw method buttons if they are on this index
						MethodsWithButtonUtility.DrawMethodButtonsAtPropertyIndex(index, m_Type, value, () => GetNextPosition());

						index++;

					}
				});
			}

		}

		/// <summary>
		/// Draws each child property.
		/// </summary>
		/// <remarks>
		/// Override this to modify the way specific properties are drawn.
		/// </remarks>
		/// <param name="property">The child property.</param>
		protected virtual void DrawChildProperty(SerializedProperty property) {
			EditorGUI.PropertyField(GetNextPosition(property), property, true);
		}

		/// <summary>
		/// Draws the property field on non-edit mode.
		/// </summary>
		/// <remarks>
		/// Override this to enhance or modify the drawing behaviour.
		/// </remarks>
		/// <param name="propertyRect">The property rect</param>
		/// <param name="guiContent">The GUIContent, used to create the label of the property.</param>
		/// <param name="name">The name of the object to display inside the field box.</param>
		protected virtual void DrawPropertyField(Rect propertyRect, GUIContent guiContent, string name) {

			GetPropertySubRects(propertyRect, out var labelRect, out var fieldRect);

			// Documentation tooltip
			if (Property.managedReferenceValue != null && !string.IsNullOrEmpty(DocumentationCommentProperty.stringValue)) {

				var documentationRect = fieldRect;
				documentationRect.xMax = fieldRect.xMin - 2;
				documentationRect.xMin = documentationRect.xMax - 20;

				GUIContent documentationGUIContent = EditorGUIUtility.IconContent("_Help@2x");
				documentationGUIContent.tooltip = DocumentationCommentProperty.stringValue;
				GUI.Button(documentationRect, documentationGUIContent, EditorStyles.iconButton);

			}

			// Create a label with the property name
			EditorGUI.LabelField(labelRect, guiContent);

			// Search for the object icon
			Texture objectIcon = GetObjectIcon(Property.managedReferenceValue?.GetType());

			// Create a box resembling an Object field
			DrawPropertyFieldBox(fieldRect, objectIcon, name);

			// Draw object icon, if any
			if (objectIcon != null) {
				var iconRect = fieldRect;
				iconRect.position += new Vector2(0, 1);
				iconRect.size = new Vector2(16, 16);
				GUI.DrawTexture(iconRect, objectIcon);
			}

		}

		/// <summary>
		/// Draws the box inside the property field.
		/// </summary>
		/// <remarks>
		/// Override this to enhance or modify the drawing behaviour.
		/// </remarks>
		/// <param name="boxRect">The Rect for the box.</param>
		/// <param name="objectIcon">The icon of the box, if any.</param>
		/// <param name="name">The name to be displayed on the box.</param>
		protected virtual void DrawPropertyFieldBox(Rect boxRect, Texture objectIcon, string name) {
			EditorGUI.BeginDisabledGroup(true);
			GUI.Box(boxRect, objectIcon != null ? $"     {name}" : name, EditorStyles.objectField);
			EditorGUI.EndDisabledGroup();
		}

		#endregion


		#region Private Static Fields

		private static Dictionary<string, Texture> s_ObjectIcons = new Dictionary<string, Texture>();

		#endregion


		#region Private Fields

		private Type m_Type;

		#endregion


		#region Private Methods

		private void DrawBreadcrums() {

			var compositeRoot = Property.serializedObject.targetObject as ICompositeRoot;

			if (compositeRoot == null) {

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
				DrawPropertyField(propertyRect, new GUIContent(Property.displayName, Property.tooltip), $"Null");
				DrawCreateButton(firstButtonRect);
			} else {
				// Edit button
				if (CDEditorUtility.GetElementIndex(Property, out var index)) {
					// Array element
					DrawPropertyField(propertyRect, $"Element {index}", $"{DisplayName()}");
					ProcessPing(propertyRect);
				} else {
					// Single instance
					DrawPropertyField(propertyRect, new GUIContent(Property.displayName, Property.tooltip), $"{DisplayName()}");
					ProcessPing(propertyRect);
				}
				DrawEditButton(firstButtonRect);
			}

			// Remove button
			DrawDeleteButton(secondButtonRect);

			string DisplayName() {
				return (Property.managedReferenceValue as CompositeObject).DisplayName;
			}

		}

		private void DrawPropertyField(Rect propertyRect, string label, string name) {
			DrawPropertyField(propertyRect, new GUIContent(label), name);
		}

		private void ProcessPing(Rect propertyRect) {

			if (Property.managedReferenceValue == null) {
				return;
			}

			GetPropertySubRects(propertyRect, out var _, out var fieldRect);

			// Ping/Open the script
			if (Event.current.type == EventType.MouseDown &&
				Event.current.button == 0 && // Left mouse button
				fieldRect.Contains(Event.current.mousePosition)) {

				Event.current.Use();

				var type = Property.managedReferenceValue.GetType();
				string[] guids = AssetDatabase.FindAssets($"{type.Name} t:MonoScript");

				foreach (string guid in guids) {

					string path = AssetDatabase.GUIDToAssetPath(guid);
					var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

					if (monoScript != null && CDEditorUtility.IsTypeInScript(monoScript, type)) {
						EditorGUIUtility.PingObject(monoScript);
						if (Event.current.clickCount == 2) {
							AssetDatabase.OpenAsset(monoScript);
						}
						break;

					}

				}

			}

		}

		private void GetPropertySubRects(Rect propertyRect, out Rect labelRect, out Rect fieldRect) {

			// Label rect
			var labelWidth = Position.width * 0.25f;
			labelRect = propertyRect;
			labelRect.width = labelWidth;

			// Field rect
			fieldRect = propertyRect;
			fieldRect.xMin += labelWidth + 2;

		}

		private void DrawCreateButton(Rect rect) {

			var canCreate = PrefabUtility.GetPrefabInstanceHandle(Property.serializedObject.targetObject) == null;
			EditorGUI.BeginDisabledGroup(!canCreate);

			var content = new GUIContent(
				"Create",
				canCreate ? "" : $"To create {Property.displayName}, open the prefab."
			);

			if (GUI.Button(rect, content)) {

				// Save the path for later because it will be used by the GenericMenu which happens later
				var pendingProperty = Property.Copy(); // Copy, just in case
				var types = SystemUtility.GetConcreteDerivedTypes(CDEditorUtility.GetPropertyType(pendingProperty));

				// Show the menu only when there are more than one types
				if (types.Count > 1) {
					var menu = new GenericMenu();
					foreach (var type in types) {
						menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(type.Name)), false, () => {
							CreateObject(type, pendingProperty);
						});
					}
					menu.ShowAsContext();
				} else {
					// When there is only one type, create the object immediatly
					CreateObject(types[0], pendingProperty);
				}

			}
			EditorGUI.EndDisabledGroup();
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

			var compositeRoot = Property.serializedObject.targetObject as ICompositeRoot;

			EditorGUI.BeginDisabledGroup(!CompositeObject.CanEnterEdit);
			if (compositeRoot != null) {
				if (GUI.Button(rect, "Edit ▸")) {
					CompositeRootEditor.SelectCompositeObject(Property.serializedObject, Property.propertyPath);
				}
			} else {
				if (GUI.Button(rect, "Edit ▾")) {
					CompositeObject.Edit = true;
				}
			}
			EditorGUI.EndDisabledGroup();

		}

		private void DrawDeleteButton(Rect rect) {

			var canAddRemove = true;
			var parentProperty = CDEditorUtility.GetParentProperty(Property);
			if (parentProperty != null) {
				for (int i = 0; i < 3; i++) {

					var parentType = CDEditorUtility.GetPropertyType(parentProperty);
					var parentIsCompositeList = SystemUtility.IsSubclassOfRawGeneric(parentType, typeof(CompositeList<>));

					if (parentIsCompositeList) {
						canAddRemove = parentProperty.FindPropertyRelative("m_CanAddRemove").boolValue;
						break;
					}

					parentProperty = CDEditorUtility.GetParentProperty(parentProperty);
					if (parentProperty == null) {
						break;
					}

				}
			}
			var thereIsInstanceAndCanBeDeleted = CompositeObject != null ? CompositeObject.CanDeleteInstance : false;
			var deleteButtonEnabled = canAddRemove && thereIsInstanceAndCanBeDeleted && PrefabUtility.GetPrefabInstanceHandle(Property.serializedObject.targetObject) == null;
			EditorGUI.BeginDisabledGroup(!deleteButtonEnabled);
			var content = new GUIContent(
				"Delete",
				(deleteButtonEnabled || Property.managedReferenceValue == null) ? "" : $"To delete {Property.displayName}, open the prefab."
			);
			if (GUI.Button(rect, content)) {
				Property.managedReferenceValue = null;
			}
			EditorGUI.EndDisabledGroup();
		}

		#endregion


	}

}