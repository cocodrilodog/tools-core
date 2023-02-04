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
			var baseHeight = base.GetEditPropertyHeight(property, label);
			return baseHeight + EditorGUI.GetPropertyHeight(PuppiesProperty) + 5;
		}

		protected override void OnEditGUI(Rect position, SerializedProperty property, GUIContent label) {
			base.OnEditGUI(position, property, label);
			var rect = GetNextPosition(); // An accurate height is not needed here
			EditorGUI.PropertyField(rect, PuppiesProperty, true);
		}

		#endregion


		#region Private Properties

		private SerializedProperty PuppiesProperty { get; set; }

		#endregion


	}

}