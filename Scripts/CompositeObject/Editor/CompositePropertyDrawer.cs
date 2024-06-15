namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using UnityEditor;
	using UnityEditorInternal;
	using UnityEngine;

	/// <summary>
	/// Base class for property drawers of <see cref="CompositeObject"/> concrete classes.
	/// </summary>
	[CustomPropertyDrawer(typeof(CompositeObject), true)]
	public class CompositePropertyDrawer : PropertyDrawerBase {


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
		/// Obtains a list of possible concrete classes that will appear in a context menu when the user
		/// clicks the "Create" button.
		/// </summary>
		protected List<Type> CompositeTypes {
			get {

				// The concrete composite object
				var concreteType = CDEditorUtility.GetPropertyType(Property);

				// Create the list
				var allConcreteSubtypes = new List<Type>();

				// Add the current type, if not abstract
				if (!concreteType.IsAbstract) {
					allConcreteSubtypes.Add(concreteType);
				}

				// Get all the types of the assembly
				var assemblyTypes = concreteType.Assembly.GetTypes();

				// Find all subtypes that are concrete. This will include grand children, great grand
				// children, etc., because it is approving all that are assignable to the concreteType
				var concreteSubtypes = assemblyTypes
					.Where(t => concreteType.IsAssignableFrom(t) && t != concreteType && !t.IsAbstract)
					.ToList();

				// Add them to the list
				allConcreteSubtypes.AddRange(concreteSubtypes);
				return allConcreteSubtypes;

			}
		}

		/// <summary>
		/// <true> when the composite object can enter edit mode.
		/// </summary>
		protected bool CanEdit => Property.managedReferenceValue != null && CompositeObject.Edit;

		/// <summary>
		/// If <true>, derived composite objects will automatically draw all of their properties.
		/// This can be overriden to <c>false</c> if you want to customize the derived property drawer.
		/// </summary>
		protected virtual bool UseDefaultDrawer => true;

		#endregion


		#region Protected Methods

		protected sealed override void InitializePropertiesForGetHeight() {
			base.InitializePropertiesForGetHeight();
			NameProperty = Property.FindPropertyRelative("m_Name");
			if (CanEdit) {
				Edit_InitializePropertiesForGetHeight();
			}
		}

		protected virtual void Edit_InitializePropertiesForGetHeight() {
			DocumentationCommentProperty = Property.FindPropertyRelative("m_DocumentationComment");
		}

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

		protected virtual void Edit_InitializePropertiesForOnGUI() { }

		/// <summary>
		/// Gets the height needed for the property when it is in edit mode.
		/// </summary>
		/// <param name="property">The property</param>
		/// <param name="label">The label</param>
		/// <returns>The height</returns>
		protected virtual float Edit_GetPropertyHeight(SerializedProperty property, GUIContent label) {
			
			float height = 0;

			var compositeRoot = property.serializedObject.targetObject as CompositeRoot;

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

			if((Property.managedReferenceValue as CompositeObject).EditDocumentationComment) {
				height += EditorGUI.GetPropertyHeight(DocumentationCommentProperty) + 2;
				height += EditorGUIUtility.singleLineHeight + 2;
				height += 10; // Give dome space before owner and reuse id
			}

			if (UseDefaultDrawer) {
				CDEditorUtility.IterateChildProperties(Property, p => {
					if (p.propertyPath != NameProperty.propertyPath) {
						height += EditorGUI.GetPropertyHeight(p) + 2;
					}
				});
			}

			return height;

		}

		/// <summary>
		/// The <c>OnGUI</c> for the edit mode.
		/// </summary>
		/// <param name="position">The position</param>
		/// <param name="property">The property</param>
		/// <param name="label">The label</param>
		protected virtual void Edit_OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			
			// This base class only handles the name property.
			var rect = GetNextPosition();
			var nameRect = rect;
			nameRect.xMax -= 22;
			EditorGUI.PropertyField(nameRect, NameProperty);

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
				CDEditorUtility.IterateChildProperties(Property, p => {
					if (p.propertyPath != NameProperty.propertyPath) {
						EditorGUI.PropertyField(GetNextPosition(p), p);
					}
				});
			}

		}

		protected virtual void DrawPropertyField(Rect propertyRect, GUIContent guiContent, string name) {

			// Label rect
			var labelWidth = Position.width * 0.25f;
			var labelRect = propertyRect;
			labelRect.width = labelWidth;

			// Field rect
			var fieldRect = propertyRect;
			fieldRect.xMin += labelWidth + 2;

			// Documentation tooltip
			if (!string.IsNullOrEmpty(DocumentationCommentProperty.stringValue)) {
			
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
			EditorGUI.BeginDisabledGroup(true);
			GUI.Box(fieldRect, objectIcon != null ? $"     {name}" : name, EditorStyles.objectField);
			EditorGUI.EndDisabledGroup();

			// Draw object icon, if any
			if(objectIcon != null) {
				var iconRect = fieldRect;
				iconRect.position += new Vector2(0, 1);
				iconRect.size = new Vector2(16, 16);
				GUI.DrawTexture(iconRect, objectIcon);
			}

		}

		#endregion


		#region Private Static Fields

		private static Dictionary<string, Texture> s_ObjectIcons = new Dictionary<string, Texture>();

		#endregion


		#region Private Properties

		private CompositeObject CompositeObject => Property.managedReferenceValue as CompositeObject;

		private SerializedProperty NameProperty { get; set; }

		private SerializedProperty DocumentationCommentProperty { get; set; }

		#endregion


		#region Private Methods

		private void DrawBreadcrums() {

			var compositeRoot = Property.serializedObject.targetObject as CompositeRoot;

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
					DrawPropertyField(propertyRect, $"Element {index}", $"{DisplayName()}");
				} else {
					DrawPropertyField(propertyRect, new GUIContent(Property.displayName, Property.tooltip), $"{DisplayName()}");
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

			var compositeRoot = Property.serializedObject.targetObject as CompositeRoot;

			if (compositeRoot != null) {
				if (GUI.Button(rect, "Edit ▸")) {
					CompositeRootEditor.SelectCompositeObject(Property.serializedObject, Property.propertyPath);
				} 
			} else {
				if (GUI.Button(rect, "Edit ▾")) {					
					CompositeObject.Edit = true;
				}
			}

		}

		private void DrawDeleteButton(Rect rect) {
			var canDelete = PrefabUtility.GetPrefabInstanceHandle(Property.serializedObject.targetObject) == null;
			EditorGUI.BeginDisabledGroup(
				Property.managedReferenceValue == null ||
				!canDelete
			);
			var content = new GUIContent(
				"Delete", 
				(canDelete || Property.managedReferenceValue == null) ? "" : $"To delete {Property.displayName}, open the prefab."
			);
			if (GUI.Button(rect, content)){
				Property.managedReferenceValue = null;
			}
			EditorGUI.EndDisabledGroup();
		}

		#endregion


	}

}