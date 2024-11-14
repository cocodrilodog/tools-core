namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// This tests how to override the drawing of individual child properties.
	/// </summary>
	[CustomPropertyDrawer(typeof(SomeObject))]
	public class SomeObjectPropertyDrawer : CDObjectPropertyDrawer {


		#region Protected Methods

		protected override void InitializePropertiesForGetHeight() {
			base.InitializePropertiesForGetHeight();
			m_StringOptions_MissingSource_Property = Property.FindPropertyRelative("StringOptions_MissingSource");
		}

		protected override void InitializePropertiesForOnGUI() {
			base.InitializePropertiesForOnGUI();
			m_StringOptions_MissingSource_Property = Property.FindPropertyRelative("StringOptions_MissingSource");
		}

		protected override float GetChildPropertyHeight(SerializedProperty property) {
			if (property.propertyPath == m_StringOptions_MissingSource_Property.propertyPath) {
				var height = EditorGUI.GetPropertyHeight(property) + 2; // Property height
				height += GetHelpBoxHeight() + 2; // Help box height
				height += EditorGUIUtility.singleLineHeight + 2; // Button height
				return height;
			} else {
				return base.GetChildPropertyHeight(property);
			}
		}

		protected override void DrawChildProperty(SerializedProperty property) {
			
			// Unity bug, sometimes I receive a value of 1 here
			m_Width = Position.width > 1 ? Position.width : m_Width;
			
			if (property.propertyPath == m_StringOptions_MissingSource_Property.propertyPath) {

				// Draw the property
				EditorGUI.PropertyField(GetNextPosition(property), property);

				// Draw a help box
				var helpRect = GetNextPosition(GetHelpBoxHeight());
				GetNextPosition(2f);
				helpRect.xMin += EditorGUI.indentLevel * 15;
				EditorGUI.HelpBox(helpRect, m_HelpMessage, MessageType.Info);

			} else {
				base.DrawChildProperty(property);
			}

		}

		#endregion


		#region Private Fields

		private SerializedProperty m_StringOptions_MissingSource_Property;

		private string m_HelpMessage = "This help box tests how to draw individual child properties by using a " +
			"property drawer that inherits from CDObjectPropertyDrawer, in this case: SomeObjectPropertyDrawer.";

		private float m_Width;

		#endregion


		#region Private Methods

		public float GetHelpBoxHeight() {
			return EditorStyles.helpBox.CalcHeight(
				new GUIContent(m_HelpMessage),
				m_Width // Start with the width of the property
				- EditorGUI.indentLevel * 15 // Subtract the indent 
				- 32 // Subtract the icon and space
			);
		}

		#endregion


	}

}