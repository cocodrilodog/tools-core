namespace CocodriloDog.Core.Examples {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(Dog))]
	public class DogPropertyDrawer : CompositePropertyDrawer {


		#region Protected Properties

		protected override List<Type> CompositeTypes {
			get {
				if (m_CompositeTypes == null) {
					m_CompositeTypes = new List<Type> {
						typeof(Dog),
						typeof(DogMother),
					};
				}
				return m_CompositeTypes;
			}
		}

		#endregion


		#region Protected Methods

		protected override void InitializePropertiesForGetHeight() {
			base.InitializePropertiesForGetHeight();
			RaceProperty = Property.FindPropertyRelative("Race");
		}

		protected override float GetEditPropertyHeight(SerializedProperty property, GUIContent label) {
			var baseHeight = base.GetEditPropertyHeight(property, label);
			return baseHeight + EditorGUI.GetPropertyHeight(RaceProperty) + 5;
		}

		//protected override void InitializePropertiesForOnGUI() {
		//	base.InitializePropertiesForOnGUI();
		//	RaceProperty = Property.FindPropertyRelative("Race");
		//}

		protected override void OnEditGUI(Rect position, SerializedProperty property, GUIContent label) {
			base.OnEditGUI(position, property, label);
			var raceRect = GetNextPosition();
			EditorGUI.PropertyField(raceRect, RaceProperty);
		}

		#endregion


		#region Private Properties

		private SerializedProperty RaceProperty { get; set; }

		#endregion


		#region Private Fields

		private List<Type> m_CompositeTypes;

		#endregion


	}

}