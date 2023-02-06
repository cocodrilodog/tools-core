namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(DogMother))]
	public class DogMotherPropertyDrawer : DogPropertyDrawer {


		#region Protected Methods

		protected override void InitializePropertiesForGetHeight() {
			base.InitializePropertiesForGetHeight();
			PuppiesProperty = Property.FindPropertyRelative("Puppies");
		}

		protected override float GetEditPropertyHeight(SerializedProperty property, GUIContent label) {
			// Start with the height calculated by the base class
			var baseHeight = base.GetEditPropertyHeight(property, label);
			// Add the needed height for this subclass
			return baseHeight + EditorGUI.GetPropertyHeight(PuppiesProperty) + 5;
		}

		protected override void OnEditGUI(Rect position, SerializedProperty property, GUIContent label) {
			base.OnEditGUI(position, property, label);
			// Next position is already where it needs to be now
			var rect = GetNextPosition(); // An accurate height required by the array elements is not needed here
			EditorGUI.PropertyField(rect, PuppiesProperty, true);
		}

		#endregion


		#region Private Properties

		private SerializedProperty PuppiesProperty { get; set; }

		#endregion


	}

}