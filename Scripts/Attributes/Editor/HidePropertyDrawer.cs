namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(HideAttribute))]
	public class HidePropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

			var defaultHeight = base.GetPropertyHeight(property, label);

			if (m_HideByPropertyPath.TryGetValue(Property.propertyPath, out bool hide)) {
				if (hide) {
					return -2;
				}
			}

			var type = CDEditorUtility.GetPropertyType(Property);

			if (SystemUtility.IsSubclassOfRawGeneric(type, typeof(ListWrapper<>))) {
				var listProperty = Property.FindPropertyRelative("m_List");
				return EditorGUI.GetPropertyHeight(listProperty, Label);
			} else {
				return defaultHeight;
			}

		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);

			Label = EditorGUI.BeginProperty(Position, Label, Property);

			object targetObject = null;
			MethodInfo method = null;
			var hideAttribute = attribute as HideAttribute;

			// Find the help method
			// First look in the parent property to support system objects

			// Try to get the parent property, which will be used to look for asset or method
			var parentProperty = CDEditorUtility.GetNonArrayOrListParentProperty(Property);

			if (parentProperty != null) {
				targetObject = CDEditorUtility.GetPropertyValue(parentProperty);
				method = CDEditorUtility.GetMethod(targetObject, hideAttribute.MethodName);
			}
			// Resort to the serializedObject.targetObject
			if (method == null) {
				targetObject = Property.serializedObject.targetObject;
				method = CDEditorUtility.GetMethod(targetObject, hideAttribute.MethodName);
			}

			// Get the results
			m_HideByPropertyPath[Property.propertyPath] = false;
			if (method != null) {
				m_HideByPropertyPath[Property.propertyPath] = (bool)method.Invoke(targetObject, null);
			}
			if (!m_HideByPropertyPath[Property.propertyPath]) {
				EditorGUI.indentLevel += hideAttribute.IndentDelta;
				var type = CDEditorUtility.GetPropertyType(Property);
				if (SystemUtility.IsSubclassOfRawGeneric(type, typeof(ListWrapper<>))) {
					// Draw the ListWrapper property
					var listProperty = Property.FindPropertyRelative("m_List");
					EditorGUI.PropertyField(GetNextPosition(listProperty), listProperty, new GUIContent(Property.displayName, Property.tooltip));
				} else {
					// Draw the normal property
					EditorGUI.PropertyField(GetNextPosition(Property), Property);
				}
				EditorGUI.indentLevel -= hideAttribute.IndentDelta;
			}

			EditorGUI.EndProperty();

		}

		#endregion


		#region Private Fields

		private Dictionary<string, bool> m_HideByPropertyPath = new Dictionary<string, bool>();

		#endregion


	}

}
