namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Property drawer for system objects that allows to draw child properties individually
	/// with <see cref="GetChildPropertyHeight(UnityEditor.SerializedProperty)"/>
	/// and <see cref="DrawChildProperty(UnityEditor.SerializedProperty)"/> and supports
	/// the <see cref="ButtonAttribute"/> on child properties.
	/// </summary>
	[CustomPropertyDrawer(typeof(CDObject), true)]
	public class CDObjectPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			
			base.GetPropertyHeight(property, label);

			// The foldout's height
			float height = EditorGUIUtility.singleLineHeight + 2;
			if (Property.isExpanded) {

				// The child properties height
				// Get the type to get the methods with button later
				if (m_Type == null) {
					m_Type = CDEditorUtility.GetPropertyType(Property);
				}

				// Iterate the properties
				var index = 0;
				CDEditorUtility.IterateChildProperties(Property, p => {

					// Add the property height
					height += GetChildPropertyHeight(p);

					// Add the height of the buttons of the methods with button
					height += MethodsWithButtonUtility.GetMethodButtonsHeightAtPropertyIndex(index, m_Type);

					index++;

				});

			}

			return height;

		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			
			base.OnGUI(position, property, label);

			// Draw the object property with its children
			Property.isExpanded = EditorGUI.PropertyField(GetNextPosition(1), Property, label, false); // Exclude default childen
			
			// Draw children differently
			EditorGUI.indentLevel++;
			if (Property.isExpanded) {

				CDEditorUtility.GetPropertyValueAndType(Property, out var value, out m_Type);
				var index = 0;
				
				CDEditorUtility.IterateChildProperties(Property, p => {
					
					// Draw the child property
					DrawChildProperty(p);

					// Draw method buttons if they are on this index
					MethodsWithButtonUtility.DrawMethodButtonsAtPropertyIndex(index, m_Type, value, () => GetNextPosition());
					
					index++;

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


		#region Private Fields

		private Type m_Type;

		#endregion


	}

}