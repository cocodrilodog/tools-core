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

			var type = CDEditorUtility.GetPropertyType(Property);
			if (SystemUtility.IsSubclassOfRawGeneric(type, typeof(ListWrapper<>))) {
				var listProperty = Property.FindPropertyRelative("m_List");
				height = EditorGUI.GetPropertyHeight(listProperty, Label);
			}
			if (m_ShouldDrawHelp) {
				height += GetMessageHeight();
			}

			return height;

		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			
			base.OnGUI(position, property, label);

			Label = EditorGUI.BeginProperty(Position, Label, Property);

			object targetObject = null;
			MethodInfo method = null;
			var helpAttribute = attribute as HelpAttribute;

			// Find the help method
			// First look in the parent property to support system objects

			// Try to get the parent property, which will be used to look for asset or method
			SerializedProperty parentProperty = CDEditorUtility.GetNonArrayOrListParentProperty(Property);

			if (parentProperty != null) {
				targetObject = CDEditorUtility.GetPropertyValue(parentProperty);
				method = CDEditorUtility.GetMethod(targetObject, helpAttribute.MethodName);
			}
			// Resort to the serializedObject.targetObject
			if (method == null) {
				targetObject = Property.serializedObject.targetObject;
				method = CDEditorUtility.GetMethod(targetObject, helpAttribute.MethodName);
			}

			// Get the results
			var currentCode = 0;
			var parameters = new object[] { null };
			if (method != null) {
				currentCode = (int)method.Invoke(targetObject, parameters);
				m_CurrentMessage = parameters[0] as string;
			}

			// Draw the property field
			var isList = false;
			var isExpandedList = false;

			EditorGUI.BeginDisabledGroup(currentCode < 0);
			var type = CDEditorUtility.GetPropertyType(Property);
			if (SystemUtility.IsSubclassOfRawGeneric(type, typeof(ListWrapper<>))) {
				// Draw the ListWrapper property
				var listProperty = Property.FindPropertyRelative("m_List");
				isList = true;
				isExpandedList = listProperty.isExpanded;
				EditorGUI.PropertyField(GetNextPosition(listProperty), listProperty, new GUIContent(Property.displayName));
			} else {
				// Draw the normal property
				EditorGUI.PropertyField(GetNextPosition(Property), Property);
			}
			EditorGUI.EndDisabledGroup();

			// Define whether to show the help box or not
			m_ShouldDrawHelp = false;
			if (isList) {
				if (isExpandedList && currentCode != 0) {
					m_ShouldDrawHelp = true;
				}
			} else if (currentCode != 0) {
				m_ShouldDrawHelp = true;
			}

			// Draw the helpbox
			if (m_ShouldDrawHelp) {
				var helpRect = GetNextPosition(GetMessageHeight());
				EditorGUI.HelpBox(helpRect, m_CurrentMessage, (MessageType)Mathf.Abs(currentCode));
				GetNextPosition(2f);
			}

			EditorGUI.EndProperty();

		}

		#endregion


		#region Private Fields

		private bool m_ShouldDrawHelp;

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
