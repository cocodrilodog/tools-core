namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(ScriptableCompositeStateMachine), true)]
	public class ScriptableCompositeStateMachineEditor : ScriptableCompositeRootEditor {


		#region Unity Methods

		protected override void OnEnable() {
			base.OnEnable();
			m_ScriptProperty = serializedObject.FindProperty("m_Script");
			m_StatesProperty = serializedObject.FindProperty("m_States");
		}

		#endregion


		#region Protected Methods

		protected override void OnRootInspectorGUI() {
			serializedObject.Update();
			CDEditorUtility.DrawDisabledField(m_ScriptProperty);
			CDEditorUtility.IterateChildProperties(serializedObject, p => {
				if (p.name != "m_Script" && p.name != "m_States") {
					EditorGUILayout.PropertyField(p);
				}
			});
			// This is just to leave the states after all other properties.
			EditorGUILayout.PropertyField(m_StatesProperty);
			serializedObject.ApplyModifiedProperties();
		}

		#endregion


		#region Private Fields

		private SerializedProperty m_ScriptProperty;

		private SerializedProperty m_StatesProperty;

		#endregion


	}

}