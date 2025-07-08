namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(CompositeObjectReference<>))]
	public class CompositeObjectReferencePropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return EditorGUIUtility.singleLineHeight + 2;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			
			base.OnGUI(position, property, label);

			Label = EditorGUI.BeginProperty(Position, Label, Property);

			// General variables
			var propertyType = CDEditorUtility.GetPropertyType(Property);
			var referencedType = propertyType.GetGenericArguments()[0];

			var centralSeparation = 24;
			var previousLabelWidth = EditorGUIUtility.labelWidth;
			var labelWidth = 100;

			// Draw the source label
			var sourceLabelRect = Position;
			sourceLabelRect.width = labelWidth;
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUI.LabelField(sourceLabelRect, Label);
			EditorGUIUtility.labelWidth = previousLabelWidth;

			// Set default source if it is null.
			if (m_SourceProperty.objectReferenceValue == null) {
				m_SourceProperty.objectReferenceValue = Property.serializedObject.targetObject;
			}

			if (m_AllowOverrideSourceProperty.boolValue) {

				// Remove the default value if override is checked
				if (m_OverrideSourceProperty.boolValue &&
					m_SourceProperty.objectReferenceValue == Property.serializedObject.targetObject) {
					m_SourceProperty.objectReferenceValue = null;
				}

				// Draw the source field
				var sourceFieldRect = Position;
				sourceFieldRect.width = sourceFieldRect.width * 0.5f - centralSeparation * 0.5f;
				sourceFieldRect.xMin += labelWidth;

				EditorGUI.BeginDisabledGroup(!m_OverrideSourceProperty.boolValue);
				EditorGUI.PropertyField(sourceFieldRect, m_SourceProperty, GUIContent.none);
				EditorGUI.EndDisabledGroup();

				// Draw override source toggle
				var overrideSourceRect = new Rect(Vector2.zero, Vector2.one * 20);
				overrideSourceRect.center = Position.center;
				EditorGUI.BeginChangeCheck();
				m_OverrideSourceProperty.boolValue = EditorGUI.Toggle(overrideSourceRect, m_OverrideSourceProperty.boolValue);
				if (EditorGUI.EndChangeCheck()) {
					// Set default source if it is null and override source was unchecked.
					if (!m_OverrideSourceProperty.boolValue && 
						m_SourceProperty.objectReferenceValue == null) {
						m_SourceProperty.objectReferenceValue = Property.serializedObject.targetObject;
					}
					return;
				}

				CDEditorGUI.DrawControlTooltip(overrideSourceRect, "Override source", Vector2.down * 10);

			}

			// Draw the value field
			var valueRect = Position;
			if (m_AllowOverrideSourceProperty.boolValue) {
				// Make it half the inspector
				valueRect.xMin += valueRect.width * 0.5f + centralSeparation;
			} else {
				// Make it normal
				valueRect.xMin += EditorGUIUtility.labelWidth;
			}

			// Draw a temp UI while other source is chosen
			if (m_OverrideSourceProperty.boolValue && m_SourceProperty.objectReferenceValue == null) {
				valueRect.yMax -= 2;
				EditorGUI.HelpBox(valueRect, "Choose a new source.", MessageType.Warning);
				EditorGUI.EndProperty();
				return;
			}

			// Get the map either from one source object, or several components in a GO.
			Dictionary<string, CompositeObject> compositeObjectsMap = new();

			// Optional storage for when the source object is a GO.
			Dictionary<string, Component> compositeObjectPathsToComponents = new();

			// The source GO, if any.
			GameObject gameObject = null;

			if (m_SourceProperty.objectReferenceValue is GameObject go) {
				gameObject = go;
			} else if (m_SourceProperty.objectReferenceValue is Component comp) {
				gameObject = comp.gameObject;
			}

			if (gameObject != null && m_OverrideSourceProperty.boolValue) {
				// The source is either a GO or a component.
				foreach (var component in gameObject.GetComponents<Component>()) {
					// Get the map of each component
					var compositeObjectsMapOfComponent = CompositeObjectMaps.GetCompositeObjectsMap(
						component,
						referencedType
					);
					// If found, add it to the main map, keyValue by keyValue.
					if(compositeObjectsMapOfComponent != null) {
						foreach(var keyValue in compositeObjectsMapOfComponent) {
							// Key is the CompositeObject path, value is the CompositeObject
							compositeObjectsMap.Add(keyValue.Key, keyValue.Value);
							// Save a reference of the component to replace the GO source, when this
							// CompositeObject is chosen.
							compositeObjectPathsToComponents.Add(keyValue.Key, component);
						}
					}
				}
			} else {
				// The source is the default Component without m_OverrideSource  or a ScriptableObject
				compositeObjectsMap = CompositeObjectMaps.GetCompositeObjectsMap(
					m_SourceProperty.objectReferenceValue,
					referencedType
				);
			}

			var options = compositeObjectsMap.Keys.ToList();
			options.Insert(0, "Null"); // Allow the first choice to be null
			options.Insert(1, "--"); // Add a separator

			// Find the key (path) of the current composite object (if any) in the map
			var currentCompositeObject = CompositeObjectMaps.GetCompositeObjectById(
				m_SourceProperty.objectReferenceValue, referencedType, m_IdProperty.stringValue
			);
			var currentPath = compositeObjectsMap.FirstOrDefault(
				pathToCompositeObject => pathToCompositeObject.Value == currentCompositeObject
			).Key;

			// Find the corresponding index in the options list
			var currentIndex = options.IndexOf(currentPath); // If currentKey == null, currentIndex will be -1
			currentIndex = currentIndex == -1 ? 0 : currentIndex; // If currentIndex == -1 choose 0 to display "Null"

			// Draw the hierarchy dropdown with the options.
			CDEditorGUI.HierarchyDropdown(Property.propertyPath, valueRect, currentPath, options, (id, newPath) => {
				compositeObjectPathsToComponents.TryGetValue(newPath, out var component);
				m_NewChoice = new CompositeObjectData(id, newPath, component);
			});

			// There is a pending new choice at m_NewPath.CompositePath that needs to be assigned to the property with path
			// m_NewPath.PropertyPath
			if (m_NewChoice != null && m_NewChoice.CompositeObjectReference_PropertyPath == Property.propertyPath) {

				// Here the source GO needs to change to the component if it was obtained from a GO
				if (m_NewChoice.ComponentSource != null) {
					m_SourceProperty.objectReferenceValue = m_NewChoice.ComponentSource;
				}

				// Assign the Id now, if it is found
				if (compositeObjectsMap.TryGetValue(m_NewChoice.CompositeObjectPath, out var newValue)) {
					m_IdProperty.stringValue = newValue.Id;
				} else {
					m_IdProperty.stringValue = null;
				}

				m_NewChoice = null;

			}

			EditorGUI.EndProperty();

		}

		#endregion


		#region Protected Methods

		protected override void InitializePropertiesForGetHeight() {
			base.InitializePropertiesForGetHeight();
			InitializeProperties();
		}

		protected override void InitializePropertiesForOnGUI() {
			base.InitializePropertiesForOnGUI();
			InitializeProperties();
		}

		#endregion


		#region Private Fields

		private SerializedProperty m_SourceProperty;

		private SerializedProperty m_OverrideSourceProperty;

		private SerializedProperty m_AllowOverrideSourceProperty;

		private SerializedProperty m_IdProperty;

		private CompositeObjectData m_NewChoice;

		#endregion


		#region Private Methods

		private void InitializeProperties() {
			m_SourceProperty = Property.FindPropertyRelative("m_Source");
			m_OverrideSourceProperty = Property.FindPropertyRelative("m_OverrideSource");
			m_AllowOverrideSourceProperty = Property.FindPropertyRelative("m_AllowOverrideSource");
			m_IdProperty = Property.FindPropertyRelative("m_Id");
		}

		#endregion


		#region Support Classes

		/// <summary>
		/// Data about a <see cref="CompositeObject"/> that is used to set a new <see cref="CompositeObjectReference{T}"/> 
		/// value asynchronously given the fact that the 
		/// <see cref="CDEditorGUI.HierarchyDropdown(string, Rect, string, List{string}, Action{string, string})"/>
		/// is asynchronous.
		/// </summary>
		public class CompositeObjectData {

			/// <summary>
			/// The property path of the <see cref="CompositeObjectReference{T}"/> used as an Id.
			/// </summary>
			public string CompositeObjectReference_PropertyPath;

			/// <summary>
			/// The path of the <see cref="CompositeObject"/>, from the root to itself.
			/// </summary>
			public string CompositeObjectPath;

			/// <summary>
			/// An optional <see cref="Component"/> used when the <see cref="m_SourceProperty"/> object is a 
			/// <see cref="GameObject"/> and needs to be changed to a <see cref="Component"/> when the user
			/// chooses a <see cref="CompositeObject"/>.
			/// </summary>
			public Component ComponentSource;

			public CompositeObjectData(
				string compositeObjectReference_propertyPath, 
				string compositeObjectPath, 
				Component componentSource
			) {
				CompositeObjectReference_PropertyPath = compositeObjectReference_propertyPath;
				CompositeObjectPath = compositeObjectPath;
				ComponentSource = componentSource;
			}

		}

		#endregion


	}

}