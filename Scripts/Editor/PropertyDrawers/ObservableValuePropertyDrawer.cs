namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ObservableValue<>), true)]
	public class ObservableValuePropertyDrawer : PropertyDrawerBase {


		#region Public Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			base.GetPropertyHeight(property, label);
			return EditorGUI.GetPropertyHeight(m_ValueProperty);
		}

		#endregion


		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			base.OnGUI(position, property, label);
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.PropertyField(
				GetNextPosition(m_ValueProperty), 
				m_ValueProperty, 
				new GUIContent(Property.displayName, Property.tooltip), 
				true
			);
			EditorGUI.EndProperty();
		}

		#endregion


		#region Protected Methods

		protected override void InitializePropertiesForGetHeight() {
			base.InitializePropertiesForGetHeight();
			m_ValueProperty = Property.FindPropertyRelative("m_Value");
		}

		protected override void InitializePropertiesForOnGUI() {
			base.InitializePropertiesForOnGUI();
			m_ValueProperty = Property.FindPropertyRelative("m_Value");
		}

		#endregion


		#region Private Fields

		private SerializedProperty m_ValueProperty;

		#endregion


	}

}