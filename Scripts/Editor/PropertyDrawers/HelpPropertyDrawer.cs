namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(HelpAttribute))]
	public class HelpPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var height = base.GetPropertyHeight(property, label);
			if (m_CurrentCode != 0) {
				height += GetMessageHeight();
			}
			return height;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			
			base.OnGUI(position, property, label);

			Label = EditorGUI.BeginProperty(Position, Label, Property);

			// Find the help method
			var targetObject = Property.serializedObject.targetObject;
			var helpAttribute = attribute as HelpAttribute;
			var method = CDEditorUtility.GetMethod(targetObject, helpAttribute.MethodName);

			// Get the results
			m_CurrentCode = 0;
			var parameters = new object[] { null };
			if (method != null) {
				m_CurrentCode = (int)method.Invoke(targetObject, parameters);
				m_CurrentMessage = parameters[0] as string;
			}

			EditorGUI.BeginDisabledGroup(m_CurrentCode < 0);
			EditorGUI.PropertyField(GetNextPosition(Property), Property);
			EditorGUI.EndDisabledGroup();

			// Draw the helpbox
			if (m_CurrentCode != 0) {
				var helpRect = GetNextPosition(GetMessageHeight());
				EditorGUI.HelpBox(helpRect, m_CurrentMessage, (MessageType)Mathf.Abs(m_CurrentCode));
				GetNextPosition(2f);
			}

			EditorGUI.EndProperty();

		}

		#endregion


		#region Private Fields

		private int m_CurrentCode;

		private string m_CurrentMessage;

		#endregion


		#region Private Methods

		private float GetMessageHeight() {
			var contentHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(m_CurrentMessage), Position.width);
			contentHeight = Mathf.Max(contentHeight + 2, (EditorGUIUtility.singleLineHeight + 2) * 2);
			return contentHeight;
		}

		#endregion


	}

}
