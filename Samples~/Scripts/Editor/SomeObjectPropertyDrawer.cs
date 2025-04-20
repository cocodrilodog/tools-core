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


		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			base.OnGUI(position, property, label);
			if (Event.current.type == EventType.Repaint) {
				m_PropertyWidth = Position.width;
			}
		}

		#endregion


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
				height += CalcHelpBoxHeight() + 2; // Help box height
				return height;
			} else {
				return base.GetChildPropertyHeight(property);
			}
		}

		protected override void DrawChildProperty(SerializedProperty property) {
			
			if (property.propertyPath == m_StringOptions_MissingSource_Property.propertyPath) {

				// Draw the property
				EditorGUI.PropertyField(GetNextPosition(property), property);

				// Draw a help box
				var helpRect = GetNextPosition(m_HelpBoxHeight);
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

		private float m_PropertyWidth;

		private float m_HelpBoxHeight;

		#endregion


		#region Private Methods

		/// <summary>
		/// Call this from <see cref="GetChildPropertyHeight"/> only
		/// </summary>
		/// <returns></returns>
		public float CalcHelpBoxHeight() {
			var height = EditorStyles.helpBox.CalcHeight(
				new GUIContent(m_HelpMessage),
				m_PropertyWidth
			);
			return m_HelpBoxHeight = height;
		}

		#endregion


	}

}