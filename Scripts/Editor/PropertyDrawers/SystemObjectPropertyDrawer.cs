namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Property drawer for system objects that allows to draw child properties individually
	/// with <see cref="GetChildPropertyHeight(UnityEditor.SerializedProperty)"/>
	/// and <see cref="DrawChildProperty(UnityEditor.SerializedProperty)"/>.
	/// </summary>
	public abstract class SystemObjectPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			
			base.GetPropertyHeight(property, label);

			// The foldout's height
			float height = EditorGUIUtility.singleLineHeight + 2;
			if (Property.isExpanded) {
				// The child properties height
				CDEditorUtility.IterateChildProperties(Property, p => {
					if (p != null) {
						height += GetChildPropertyHeight(p);
					}
				});
			}

			return height;

		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			
			base.OnGUI(position, property, label);

			Property.isExpanded = EditorGUI.Foldout(GetNextPosition(1), Property.isExpanded, label, true);
			EditorGUI.indentLevel++;
			if (Property.isExpanded) {
				CDEditorUtility.IterateChildProperties(Property, p => {
					if (p != null) {
						DrawChildProperty(p);
					}
				});
			}
			EditorGUI.indentLevel--;

		}

		#endregion


		#region Protected Methods

		protected virtual float GetChildPropertyHeight(SerializedProperty property) {
			return EditorGUI.GetPropertyHeight(property, true) + 2;
		}

		protected virtual void DrawChildProperty(SerializedProperty property) {
			EditorGUI.PropertyField(GetNextPosition(property), property, true);
		}

		#endregion


	}

}