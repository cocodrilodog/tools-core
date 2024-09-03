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
			if (m_Hide) {
				return 0;
			} else {
				return base.GetPropertyHeight(property, label);
			}
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);

			Label = EditorGUI.BeginProperty(Position, Label, Property);

			// Find the hide method
			var targetObject = Property.serializedObject.targetObject;
			var hideAttribute = attribute as HideAttribute;
			var method = CDEditorUtility.GetMethod(targetObject, hideAttribute.MethodName);

			// Get the results
			m_Hide = false;
			if (method != null) {
				m_Hide = (bool)method.Invoke(targetObject, null);
			}
			if (!m_Hide) {
				EditorGUI.indentLevel += hideAttribute.IndentDelta;
				EditorGUI.PropertyField(GetNextPosition(Property), Property); ;
				EditorGUI.indentLevel -= hideAttribute.IndentDelta;
			}

			EditorGUI.EndProperty();

		}

		#endregion


		#region Private Fields

		private bool m_Hide;

		#endregion


	}

}
