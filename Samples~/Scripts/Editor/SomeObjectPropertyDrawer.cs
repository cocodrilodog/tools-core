namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// This tests how to override the drawing of individual child properties.
	/// </summary>
	[CustomPropertyDrawer(typeof(SomeObject))]
	public class SomeObjectPropertyDrawer : SystemObjectPropertyDrawer {


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
				height += GetHelpBoxHeight();
				height += EditorGUIUtility.singleLineHeight + 2; // Button height
				return height;
			} else {
				return base.GetChildPropertyHeight(property);
			}
		}

		protected override void DrawChildProperty(SerializedProperty property) {
			if(property.propertyPath == m_StringOptions_MissingSource_Property.propertyPath) {

				// Draw the property
				EditorGUI.PropertyField(GetNextPosition(property), property);

				// Draw a help box
				var helpRect = GetNextPosition(GetHelpBoxHeight());
				GetNextPosition(2f);
				helpRect.xMin += EditorGUI.indentLevel * 15;
				EditorGUI.HelpBox(helpRect, m_HelpMessage, MessageType.Info);

				// Draw a button
				var buttonRect = GetNextPosition(1);
				buttonRect.xMin += EditorGUI.indentLevel * 15;
				if (GUI.Button(buttonRect, "A Button")) {
					Debug.Log("A Button was clicked!");
				}

			} else {
				base.DrawChildProperty(property);
			}
		}

		#endregion


		#region Private Fields

		private SerializedProperty m_StringOptions_MissingSource_Property;

		private string m_HelpMessage = "This tests how to draw individual child properties by using a " +
			"property drawer that inherits from SystemObjectPropertyDrawer, in this case: SomeObjectPropertyDrawer." +
			"\nThe [Button] attribute is not supported in System.Object derivative classes, so the " +
			"button below is drawn by SomeObjectPropertyDrawer.";

		#endregion


		#region Private Methods

		public float GetHelpBoxHeight() {
			return EditorStyles.helpBox.CalcHeight(
				new GUIContent(m_HelpMessage),
				Position.width // Start with the width of the property
				- EditorGUI.indentLevel * 15 // Subtract the indent 
				- 32 // Subtract the icon and space
			);
		}

		#endregion


	}

}