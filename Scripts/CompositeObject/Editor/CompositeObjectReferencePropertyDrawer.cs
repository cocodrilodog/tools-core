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

			if (m_AllowOverrideSourceProperty.boolValue) {

				// Draw the source field
				var sourceFieldRect = Position;
				sourceFieldRect.width = sourceFieldRect.width * 0.5f - centralSeparation * 0.5f;
				sourceFieldRect.xMin += labelWidth;

				EditorGUI.BeginDisabledGroup(!m_OverrideSourceProperty.boolValue);
				EditorGUI.PropertyField(sourceFieldRect, m_SourceProperty, GUIContent.none);
				EditorGUI.EndDisabledGroup();

				// Set default source if it is null and not overriden
				if (m_SourceProperty.objectReferenceValue == null || !m_OverrideSourceProperty.boolValue) {
					m_SourceProperty.objectReferenceValue = Property.serializedObject.targetObject;
				}

				// Draw override source toggle
				var overrideSourceRect = new Rect(Vector2.zero, Vector2.one * 20);
				overrideSourceRect.center = Position.center;
				m_OverrideSourceProperty.boolValue = EditorGUI.Toggle(overrideSourceRect, m_OverrideSourceProperty.boolValue);
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

			// Remove the default value if override is checked and draw a temp UI while other source is chosen
			if (m_OverrideSourceProperty.boolValue && m_SourceProperty.objectReferenceValue == Property.serializedObject.targetObject) {
				m_SourceProperty.objectReferenceValue = null;
				valueRect.yMax -= 2;
				EditorGUI.HelpBox(valueRect, "Choose a new source.", MessageType.Warning);
				EditorGUI.EndProperty();
				return;
			}

			// Get the map of the existing composite objects (referencedType) on the source object
			var compositeObjectsMap = CompositeObjectMaps.GetCompositeObjectsMap(
				m_SourceProperty.objectReferenceValue, 
				referencedType
			);
			var options = compositeObjectsMap.Keys.ToList();
			options.Insert(0, "Null"); // Allow the first choice to be null

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
				m_NewPath = new CompositeObjectPath(id, newPath);
			});

			// There is a pending new path m_NewPath.CompositePath that needs to be assigned to property at path
			// m_NewPath.PropertyPath
			if (m_NewPath != null && m_NewPath.PropertyPath == Property.propertyPath) {

				Undo.RecordObject(m_SourceProperty.objectReferenceValue, $"Modify {Property.displayName}");
				if (compositeObjectsMap.TryGetValue(m_NewPath.CompositePath, out var newValue)) {
					//m_ValueProperty.managedReferenceValue = newValue;
					m_IdProperty.stringValue = newValue.Id;
				} else {
					//m_ValueProperty.managedReferenceValue = null;
					m_IdProperty.stringValue = null;
				}
				EditorUtility.SetDirty(m_SourceProperty.objectReferenceValue);

				m_NewPath = null;

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

		private CompositeObjectPath m_NewPath;

		#endregion


		#region Support Classes

		public class CompositeObjectPath {

			public string PropertyPath;

			public string CompositePath;

			public CompositeObjectPath(string propertyPath, string compositePath) {
				PropertyPath = propertyPath;
				CompositePath = compositePath;
			}

		}

		#endregion


		#region Private Fields

		private void InitializeProperties() {
			m_SourceProperty = Property.FindPropertyRelative("m_Source");
			m_OverrideSourceProperty = Property.FindPropertyRelative("m_OverrideSource");
			m_AllowOverrideSourceProperty = Property.FindPropertyRelative("m_AllowOverrideSource");
			m_IdProperty = Property.FindPropertyRelative("m_Id");
		}

		#endregion


	}

}