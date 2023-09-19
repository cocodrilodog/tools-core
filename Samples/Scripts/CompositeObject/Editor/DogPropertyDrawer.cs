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

		protected override void Edit_InitializePropertiesForGetHeight() {
			base.Edit_InitializePropertiesForGetHeight();
			RaceProperty = Property.FindPropertyRelative("Race");
		}

		protected override float Edit_GetPropertyHeight(SerializedProperty property, GUIContent label) {
			// Start with the height calculated by the base class
			var baseHeight = base.Edit_GetPropertyHeight(property, label);
			// Add the needed height for this subclass
			return baseHeight + EditorGUI.GetPropertyHeight(RaceProperty) + 5;
		}

		protected override void Edit_OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			base.Edit_OnGUI(position, property, label);
			// Next position is already where it needs to be now
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