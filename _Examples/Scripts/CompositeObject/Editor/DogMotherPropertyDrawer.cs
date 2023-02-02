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
			if (PuppiesProperty.isExpanded) {
				return baseHeight + GetPuppiesHeight();
			} else {
				return baseHeight + FieldHeight;
			}
		}

		protected override void OnEditGUI(Rect position, SerializedProperty property, GUIContent label) {
			base.OnEditGUI(position, property, label);
			var rect = GetNextPosition();
			rect.height = FieldHeight * (PuppiesProperty.arraySize + 3);
			//Debug.Log($"???: {PuppiesProperty.serializedObject}");
			EditorGUI.PropertyField(rect, PuppiesProperty, true);
		}

		#endregion


		#region Private Properties

		private SerializedProperty PuppiesProperty { get; set; }

		#endregion


		#region Private Methods

		private float GetPuppiesHeight() {
			// A space for the header + spaces for the items + 2 extra spaces
			return FieldHeight +
				(FieldHeight * Mathf.Max(1, PuppiesProperty.arraySize)) +
				FieldHeight * 2;
		}

		#endregion


	}

}