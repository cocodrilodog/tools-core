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

			var previousLabelWidth = EditorGUIUtility.labelWidth;
			var labelWidth = m_ShowSourceFieldProperty.boolValue ? previousLabelWidth * 0.7f : previousLabelWidth;

			// Draw the source label
			var sourceLabelRect = Position;
			sourceLabelRect.width = labelWidth;
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUI.LabelField(sourceLabelRect, Label);
			EditorGUIUtility.labelWidth = previousLabelWidth;

			// Draw the source field
			if (m_ShowSourceFieldProperty.boolValue) {

				var sourceFieldRect = Position;
				sourceFieldRect.width = sourceFieldRect.width * 0.5f - 2;
				sourceFieldRect.xMin += labelWidth;

				EditorGUI.BeginDisabledGroup(!m_EnableSourceFieldProperty.boolValue);
				EditorGUI.PropertyField(sourceFieldRect, m_SourceProperty, GUIContent.none);
				EditorGUI.EndDisabledGroup();

				if (m_SourceProperty.objectReferenceValue == null) {
					CDEditorGUI.DrawControlTooltip(sourceFieldRect, "Choose a source object with\nCompositeObject instances.", new Vector2(60, -24));
				}

			}

			// Draw the value field
			var valueRect = Position;
			if (m_ShowSourceFieldProperty.boolValue) {
				// Make it half the inspector
				valueRect.xMin += valueRect.width * 0.5f;
			} else {
				// Make it normal
				valueRect.xMin += EditorGUIUtility.labelWidth;
			}

			// Draw a temp UI while no source is chosen
			if (m_SourceProperty.objectReferenceValue == null) {
				EditorGUI.HelpBox(valueRect, "Choose a source.", MessageType.Warning);
				EditorGUI.EndProperty();
				return;
			}

			// Get the map either from one source object, or several MonoBehaviours in a GO.
			Dictionary<string, CompositeObject> compositeObjectsByPath = new();

			// Optional storage for when the source object is a GO.
			Dictionary<string, MonoBehaviour> monoBehavioursByCompositeObjectPath = new();

			// The source GO, if any.
			GameObject gameObject = null;

			if (m_SourceProperty.objectReferenceValue is GameObject go) {
				gameObject = go;
			} else if (m_SourceProperty.objectReferenceValue is Component comp) {
				gameObject = comp.gameObject;
			}

			// It is a game object 
			if (gameObject != null) {
				
				var repeatedMonoBehaviourTypesCount = new Dictionary<Type, int>();
			
				// The source is either a GO or a Component.
				// Search only on MonoBehaviours for optimal processing
				foreach (var monoBehaviour in gameObject.GetComponents<MonoBehaviour>()) {

					// Check for repeated types
					if (!repeatedMonoBehaviourTypesCount.TryAdd(monoBehaviour.GetType(), 0)) {
						repeatedMonoBehaviourTypesCount[monoBehaviour.GetType()]++;
					}

					// Get the map of each MonoBehaviour
					var monoBehaviour_compositeObjectsByPath = CompositeObjectMaps.GetCompositeObjectsMap(
						monoBehaviour,
						referencedType
					);

					// If found, add it to the main map, keyValue by keyValue.
					if(monoBehaviour_compositeObjectsByPath != null) {
						foreach(var path_compositeObject in monoBehaviour_compositeObjectsByPath) {
							
							// Key is the CompositeObject path, value is the CompositeObject
							var path = path_compositeObject.Key;

							// This happens when a component is repeated
							if (repeatedMonoBehaviourTypesCount[monoBehaviour.GetType()] >= 1) {
								int slashIndex = path.IndexOf('/');
								if (slashIndex >= 0) {
									// Insert a number on the monobehaviour part of the path
									path = path.Insert(slashIndex, $" ({repeatedMonoBehaviourTypesCount[monoBehaviour.GetType()]})");
								}
							}

							compositeObjectsByPath.Add(path, path_compositeObject.Value);
							// Save a reference of the MonoBehaviour to replace the GO source, when this
							// CompositeObject is chosen.
							monoBehavioursByCompositeObjectPath.Add(path, monoBehaviour);

						}
					}
				}

			} else {
				// It is a ScriptableObject
				compositeObjectsByPath = CompositeObjectMaps.GetCompositeObjectsMap(
					m_SourceProperty.objectReferenceValue,
					referencedType
				);
			}

			// This can be null if there is no source
			if (compositeObjectsByPath == null) {
				EditorGUI.EndProperty();
				return;
			}

			// Draw the value field
			var options = compositeObjectsByPath.Keys.ToList();
			options.Insert(0, "Null"); // Allow the first choice to be null
			options.Insert(1, "--"); // Add a separator

			// Find the key (path) of the current composite object (if any) in the map
			var currentCompositeObject = CompositeObjectMaps.GetCompositeObjectById(
				m_SourceProperty.objectReferenceValue, referencedType, m_IdProperty.stringValue
			);
			var currentPath = compositeObjectsByPath.FirstOrDefault(
				pathToCompositeObject => pathToCompositeObject.Value == currentCompositeObject
			).Key;

			// Find the corresponding index in the options list
			var currentIndex = options.IndexOf(currentPath); // If currentKey == null, currentIndex will be -1
			currentIndex = currentIndex == -1 ? 0 : currentIndex; // If currentIndex == -1 choose 0 to display "Null"

			// Draw the hierarchy dropdown with the options.
			CDEditorGUI.HierarchyDropdown(Property.propertyPath, valueRect, currentPath, options, (id, newPath) => {
				monoBehavioursByCompositeObjectPath.TryGetValue(newPath, out var component);
				m_NewChoice = new CompositeObjectData(id, newPath, component);
			});

			// There is a pending new choice at m_NewPath.CompositePath that needs to be assigned to the property with path
			// m_NewPath.PropertyPath
			if (m_NewChoice != null && m_NewChoice.CompositeObjectReference_PropertyPath == Property.propertyPath) {

				// Here the source GO needs to change to the component if it was obtained from a GO
				if (m_NewChoice.MonoBehaviour != null) {
					m_SourceProperty.objectReferenceValue = m_NewChoice.MonoBehaviour;
				}

				// Assign the Id now, if it is found
				if (compositeObjectsByPath.TryGetValue(m_NewChoice.CompositeObjectPath, out var newValue)) {
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

		private SerializedProperty m_ShowSourceFieldProperty;

		private SerializedProperty m_EnableSourceFieldProperty;

		private SerializedProperty m_IdProperty;

		private CompositeObjectData m_NewChoice;

		#endregion


		#region Private Methods

		private void InitializeProperties() {
			m_SourceProperty						= Property.FindPropertyRelative("m_Source");
			m_ShowSourceFieldProperty				= Property.FindPropertyRelative("m_ShowSourceField");
			m_EnableSourceFieldProperty				= Property.FindPropertyRelative("m_EnableSourceField");
			m_IdProperty							= Property.FindPropertyRelative("m_Id");
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
			/// An optional <see cref="UnityEngine.MonoBehaviour"/> used when the <see cref="m_SourceProperty"/> object is a 
			/// <see cref="GameObject"/> and needs to be changed to a <see cref="UnityEngine.MonoBehaviour"/> when the user
			/// chooses a <see cref="CompositeObject"/>.
			/// </summary>
			public Component MonoBehaviour;

			public CompositeObjectData(
				string compositeObjectReference_propertyPath, 
				string compositeObjectPath,
				MonoBehaviour monoBehaviour
			) {
				CompositeObjectReference_PropertyPath = compositeObjectReference_propertyPath;
				CompositeObjectPath = compositeObjectPath;
				MonoBehaviour = monoBehaviour;
			}

		}

		#endregion


	}

}