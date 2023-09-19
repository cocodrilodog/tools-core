namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(DogMother))]
	public class DogMotherPropertyDrawer : DogPropertyDrawer {


		#region Protected Methods

		protected override void Edit_InitializePropertiesForGetHeight() {
			base.Edit_InitializePropertiesForGetHeight();
			PuppiesProperty = Property.FindPropertyRelative("Puppies");
			MaleProperty = Property.FindPropertyRelative("Male");
		}

		protected override float Edit_GetPropertyHeight(SerializedProperty property, GUIContent label) {
			// Start with the height calculated by the base class
			var height = base.Edit_GetPropertyHeight(property, label);
			// Add the needed height for this subclass
			height += EditorGUI.GetPropertyHeight(PuppiesProperty) + 2;
			height += EditorGUI.GetPropertyHeight(MaleProperty) + 2;
			return height;
		}

		protected override void Edit_OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			base.Edit_OnGUI(position, property, label);
			// Next position is already where it needs to be now
			EditorGUI.PropertyField(GetNextPosition(PuppiesProperty), PuppiesProperty, true);
			EditorGUI.PropertyField(GetNextPosition(MaleProperty), MaleProperty, true);
		}

		#endregion


		#region Private Properties

		private SerializedProperty PuppiesProperty { get; set; }

		private SerializedProperty MaleProperty { get; set; }

		#endregion


	}

}