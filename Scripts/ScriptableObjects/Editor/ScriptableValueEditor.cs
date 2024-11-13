namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.UIElements;

	[CustomEditor(typeof(ScriptableValue<>), true)]
	public class ScriptableValueEditor : ScriptableEditorWithButtons {


		#region Unity Methods

		protected override void OnEnable() {
			base.OnEnable();
			m_DocumentationCommentProperty = serializedObject.FindProperty("m_DocumentationComment");
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			// This doesn't need to update the serialized object.
			DrawDocumentationHelpBox();
		}

		#endregion


		#region Private Fields

		private SerializedProperty m_DocumentationCommentProperty;

		#endregion


		#region Protected Methods

		protected override void DrawProperty(SerializedProperty property) {
			if (property.propertyPath == m_DocumentationCommentProperty.propertyPath) {
				var scriptableValue = serializedObject.targetObject as ScriptableValue;
				if (scriptableValue != null && scriptableValue.EditDocumentationComment) {
					EditorGUILayout.PropertyField(m_DocumentationCommentProperty);
					if (GUILayout.Button("Done")) {
						scriptableValue.EditDocumentationComment = false;
					}
				}
			} else {
				base.DrawProperty(property);
			}
		}

		#endregion


		#region Private Methods

		private void DrawDocumentationHelpBox() {

			var scriptableValue = serializedObject.targetObject as ScriptableValue;
			
			if (scriptableValue != null && !scriptableValue.EditDocumentationComment) {
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.HelpBox(m_DocumentationCommentProperty.stringValue, MessageType.Info, true);
					EditorGUILayout.BeginVertical(GUILayout.Width(16));
					{
						GUILayout.FlexibleSpace();
						GUIContent helpIcon = EditorGUIUtility.IconContent("_Help@2x");
						helpIcon.tooltip = "Click to edit";
						if (GUILayout.Button(helpIcon, EditorStyles.iconButton, GUILayout.Height(16))) {
							scriptableValue.EditDocumentationComment = true;
						}
						GUILayout.FlexibleSpace();
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
			}

		}

		#endregion


	}

}