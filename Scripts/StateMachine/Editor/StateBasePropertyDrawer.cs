namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(StateBase), true)]
	public class StateBasePropertyDrawer : CompositeObjectPropertyDrawer {


		#region Protected Properties

		protected override bool UseDefaultDrawer => false;

		#endregion


		#region Protected Methods

		protected override void Edit_InitializePropertiesForGetHeight() {
			base.Edit_InitializePropertiesForGetHeight();
			OnEnterProperty = Property.FindPropertyRelative("m_OnEnter");
			OnExitProperty = Property.FindPropertyRelative("m_OnExit");
		}

		protected override float Edit_GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var height = base.Edit_GetPropertyHeight(property, label);
			CDEditorUtility.IterateChildProperties(Property, p => {
				if (p.propertyPath != NameProperty.propertyPath &&
					p.propertyPath != DocumentationCommentProperty.propertyPath) {
					height += EditorGUI.GetPropertyHeight(p) + 2;
				}
			});
			return height;
		}

		protected override void Edit_InitializePropertiesForOnGUI() {
			base.Edit_InitializePropertiesForOnGUI();
			OnEnterProperty = Property.FindPropertyRelative("m_OnEnter");
			OnExitProperty = Property.FindPropertyRelative("m_OnExit");
		}

		protected override void Edit_OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			base.Edit_OnGUI(position, property, label);
			CDEditorUtility.IterateChildProperties(Property, p => {
				if (p.propertyPath != NameProperty.propertyPath &&
					p.propertyPath != DocumentationCommentProperty.propertyPath &&
					p.propertyPath != OnEnterProperty.propertyPath &&
					p.propertyPath != OnExitProperty.propertyPath) {
					EditorGUI.PropertyField(GetNextPosition(p), p);
				}
			});
			EditorGUI.PropertyField(GetNextPosition(OnEnterProperty), OnEnterProperty);
			EditorGUI.PropertyField(GetNextPosition(OnExitProperty), OnExitProperty);
		}

		#endregion


		#region Private Properties

		protected SerializedProperty OnEnterProperty;

		protected SerializedProperty OnExitProperty;

		#endregion


	}

}