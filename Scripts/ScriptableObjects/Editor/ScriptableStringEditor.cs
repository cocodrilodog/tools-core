namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(ScriptableString))]
	public class ScriptableStringEditor : ScriptableValueEditor {


		#region Unity Methods

		protected override void OnEnable() {
			base.OnEnable();
			m_ValueProperty = serializedObject.FindProperty("m_Value");
		}

		#endregion


		#region Protected Methods

		protected override void DrawProperty(SerializedProperty property) {
			if (property.propertyPath == m_ValueProperty.propertyPath) {
				EditorGUILayout.LabelField(m_ValueProperty.displayName);
				GUIStyle wordWrappedStyle = new GUIStyle(EditorStyles.textArea);
				wordWrappedStyle.wordWrap = true;
				m_ValueProperty.stringValue = EditorGUILayout.TextArea(m_ValueProperty.stringValue, wordWrappedStyle);
			} else {
				base.DrawProperty(property);
			}
		}

		#endregion


		#region Private Fields

		private SerializedProperty m_ValueProperty;

		#endregion


	}

}