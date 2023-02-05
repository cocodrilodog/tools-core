namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	public abstract class CompositeRootEditor : Editor {


		#region Unity Methods

		protected virtual void OnEnable() {
			ScriptProperty = serializedObject.FindProperty("m_Script");
			SelectedCompositePathProperty = serializedObject.FindProperty("m_SelectedCompositePath");
		}

		public sealed override void OnInspectorGUI() {
			if (string.IsNullOrEmpty(SelectedCompositePathProperty.stringValue)) {
				OnRootInspectorGUI();
			} else {
				serializedObject.Update();
				CDEditorUtility.DrawDisabledField(ScriptProperty);
				var selectedCompositeProperty = serializedObject.FindProperty(SelectedCompositePathProperty.stringValue);
				if (selectedCompositeProperty != null) {
					EditorGUILayout.PropertyField(selectedCompositeProperty);
				}
				serializedObject.ApplyModifiedProperties();
			}
		}

		#endregion


		#region Protected Methods

		public virtual void OnRootInspectorGUI() => base.OnInspectorGUI();

		#endregion


		#region Private Properties

		private SerializedProperty ScriptProperty { get; set; }

		private SerializedProperty SelectedCompositePathProperty { get; set; }

		#endregion


	}

}