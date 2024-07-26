namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(CollisionReactionBase), true)]
	public class CollisionReactionPropertyDrawer : CompositeObjectPropertyDrawer {


		#region Protected Properties

		protected override bool UseDefaultDrawer => false;

		#endregion


		#region Protected Methods

		protected override void Edit_InitializePropertiesForGetHeight() {
			base.Edit_InitializePropertiesForGetHeight();
			OtherTagProperty = Property.FindPropertyRelative("m_OtherTag");
			OnTriggerEnterProperty = Property.FindPropertyRelative("m_OnTriggerEnter");
			OnTriggerExitProperty = Property.FindPropertyRelative("m_OnTriggerExit");
			OnCollisionEnterProperty = Property.FindPropertyRelative("m_OnCollisionEnter");
			OnCollisionExitProperty = Property.FindPropertyRelative("m_OnCollisionExit");
		}

		protected override void Edit_InitializePropertiesForOnGUI() {
			base.Edit_InitializePropertiesForOnGUI();
			OtherTagProperty = Property.FindPropertyRelative("m_OtherTag");
			OnTriggerEnterProperty = Property.FindPropertyRelative("m_OnTriggerEnter");
			OnTriggerExitProperty = Property.FindPropertyRelative("m_OnTriggerExit");
			OnCollisionEnterProperty = Property.FindPropertyRelative("m_OnCollisionEnter");
			OnCollisionExitProperty = Property.FindPropertyRelative("m_OnCollisionExit");
		}

		protected override float Edit_GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var height = base.Edit_GetPropertyHeight(property, label);
			height += EditorGUI.GetPropertyHeight(OtherTagProperty) + 2;
			height += EditorGUI.GetPropertyHeight(OnTriggerEnterProperty) + 2;
			height += EditorGUI.GetPropertyHeight(OnTriggerExitProperty) + 2;
			height += EditorGUI.GetPropertyHeight(OnCollisionEnterProperty) + 2;
			height += EditorGUI.GetPropertyHeight(OnCollisionExitProperty) + 2;
			return height;
		}

		protected override void Edit_OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			
			base.Edit_OnGUI(position, property, label);
			
			EditorGUI.PropertyField(GetNextPosition(OtherTagProperty), OtherTagProperty);
			if (string.IsNullOrEmpty(OtherTagProperty.stringValue)) {
				// [Choose a Tag]
				NameProperty.stringValue = CompositeObject.DefaultName;
			} else {
				// Use the tag as a name.
				NameProperty.stringValue = OtherTagProperty.stringValue;
			}

			EditorGUI.PropertyField(GetNextPosition(OnTriggerEnterProperty), OnTriggerEnterProperty);
			EditorGUI.PropertyField(GetNextPosition(OnTriggerExitProperty), OnTriggerExitProperty);
			EditorGUI.PropertyField(GetNextPosition(OnCollisionEnterProperty), OnCollisionEnterProperty);
			EditorGUI.PropertyField(GetNextPosition(OnCollisionExitProperty), OnCollisionExitProperty);

		}

		#endregion


		#region Private Properties

		private SerializedProperty OtherTagProperty { get; set; }

		private SerializedProperty OnTriggerEnterProperty { get; set; }

		private SerializedProperty OnTriggerExitProperty { get; set; }

		private SerializedProperty OnCollisionEnterProperty { get; set; }

		private SerializedProperty OnCollisionExitProperty { get; set; }

		#endregion


	}

}