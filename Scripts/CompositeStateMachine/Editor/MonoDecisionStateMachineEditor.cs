namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using CocodriloDog.Core;

	[CustomEditor(typeof(MonoDecisionStateMachine), true)]
	public class MonoDecisionStateMachineEditor : MonoCompositeRootEditor {


		#region Unity Methods

		protected override void OnEnable() {
			base.OnEnable();
			m_ScriptProperty = serializedObject.FindProperty("m_Script");
			m_StatesProperty = serializedObject.FindProperty("m_States");
			m_TriggersProperty = serializedObject.FindProperty("m_Triggers");
		}

		#endregion


		#region Protected Methods

		protected override void OnRootInspectorGUI() {
			serializedObject.Update();
			CDEditorUtility.DrawDisabledField(m_ScriptProperty);
			CDEditorUtility.IterateChildProperties(serializedObject, p => {
				if (p.name != "m_Script" && p.name != "m_States" && p.name != "m_Triggers") {
					EditorGUILayout.PropertyField(p);
				}
			});
			// This is just to leave the states and triggers after all other properties.
			EditorGUILayout.PropertyField(m_StatesProperty);
			EditorGUILayout.PropertyField(m_TriggersProperty);
			serializedObject.ApplyModifiedProperties();
		}

		#endregion


		#region Private Fields

		private SerializedProperty m_ScriptProperty;

		private SerializedProperty m_StatesProperty;

		private SerializedProperty m_TriggersProperty;

		#endregion


	}

}