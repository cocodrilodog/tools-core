namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(ScriptableString))]
	public class ScriptableStringEditor : Editor {


		#region Unity Methods

		private void OnEnable() {
			m_ScriptProperty = serializedObject.FindProperty("m_Script");
			m_ResetOnEditModeProperty = serializedObject.FindProperty("m_ResetOnEditMode");
			m_ValueProperty = serializedObject.FindProperty("m_Value");
		}

		public override void OnInspectorGUI() {

			serializedObject.Update();

			CDEditorUtility.DrawDisabledField(m_ScriptProperty);
			EditorGUILayout.PropertyField(m_ResetOnEditModeProperty);
			EditorGUILayout.LabelField(m_ValueProperty.displayName);
			m_ValueProperty.stringValue = EditorGUILayout.TextArea(m_ValueProperty.stringValue);

			serializedObject.ApplyModifiedProperties();

		}

		#endregion


		#region Private Fields

		private SerializedProperty m_ScriptProperty;

		private SerializedProperty m_ResetOnEditModeProperty;

		private SerializedProperty m_ValueProperty;

		#endregion


	}

}