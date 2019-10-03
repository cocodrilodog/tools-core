namespace CocodriloDog.Core {

	using UnityEngine;
	using UnityEditor;
	using System.Collections;
	using System.Collections.Generic;

	[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
	public class MinMaxRangePropertyDrawer : PropertyDrawer {


		#region Editor Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			if (property.type != "MinMaxRange")
				Debug.LogWarning("Use only with MinMaxRange type");
			else {

				label = EditorGUI.BeginProperty(position, label, property);

				// Property variables
				MinMaxRangeAttribute minMaxRangeAttribute = attribute as MinMaxRangeAttribute;
				SerializedProperty minValueProperty = property.FindPropertyRelative("m_MinValue");
				SerializedProperty maxValueProperty = property.FindPropertyRelative("m_MaxValue");

				// Label
				Rect labelPosition = position;
				labelPosition.height = EditorGUIUtility.singleLineHeight;
				EditorGUI.LabelField(labelPosition, label);

				// Min Max Controls
				Rect minMaxControlsRect = labelPosition;
				minMaxControlsRect.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.indentLevel++;
				minMaxControlsRect = EditorGUI.IndentedRect(minMaxControlsRect);
				EditorGUI.indentLevel--;

				DrawMinMaxControls(minMaxControlsRect, minMaxRangeAttribute, minValueProperty, maxValueProperty);

				ClampValuesToLimits(minMaxRangeAttribute, minValueProperty, maxValueProperty);

				EditorGUI.EndProperty();

			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return base.GetPropertyHeight(property, label) * 2;
		}

		#endregion


		#region Internal Methods

		private void DrawMinMaxControls(
			Rect position,
			MinMaxRangeAttribute minMaxRangeAttribute,
			SerializedProperty minValueProperty,
			SerializedProperty maxValueProperty
		) {

			float floatFieldWidth = position.width * 0.2f;

			Rect minFloatFieldPosition = position;
			minFloatFieldPosition.width = floatFieldWidth - 4;
			DrawMinFloat(minFloatFieldPosition, minValueProperty, maxValueProperty);

			Rect maxFloatFieldPosition = position;
			maxFloatFieldPosition.x = position.xMax - floatFieldWidth + 4;
			maxFloatFieldPosition.width = floatFieldWidth - 4;
			DrawMaxFloat(maxFloatFieldPosition, minValueProperty, maxValueProperty);

			Rect sliderPosition = position;
			sliderPosition.x += floatFieldWidth;
			sliderPosition.width -= floatFieldWidth * 2;
			DrawMinMaxSlider(sliderPosition, minMaxRangeAttribute, minValueProperty, maxValueProperty);

		}

		private void DrawMinFloat(
			Rect position, SerializedProperty minValueProperty, SerializedProperty maxValueProperty
		) {
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position, minValueProperty, GUIContent.none);
			if (EditorGUI.EndChangeCheck()) {
				if (minValueProperty.floatValue > maxValueProperty.floatValue) {
					minValueProperty.floatValue = maxValueProperty.floatValue;
				}
			}
		}

		private void DrawMaxFloat(
			Rect position, SerializedProperty minValueProperty, SerializedProperty maxValueProperty
		) {
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position, maxValueProperty, GUIContent.none);
			if (EditorGUI.EndChangeCheck()) {
				if (maxValueProperty.floatValue < minValueProperty.floatValue) {
					maxValueProperty.floatValue = minValueProperty.floatValue;
				}
			}
		}

		private void DrawMinMaxSlider(
			Rect position, 
			MinMaxRangeAttribute minMaxRangeAttribute,
			SerializedProperty minValueProperty,
			SerializedProperty maxValueProperty
		) {

			float newMinValue = minValueProperty.floatValue;
			float newMaxValue = maxValueProperty.floatValue;

			EditorGUI.BeginChangeCheck();
			EditorGUI.MinMaxSlider(
				position,
				ref newMinValue, ref newMaxValue,
				minMaxRangeAttribute.MinLimit, minMaxRangeAttribute.MaxLimit
			);

			if (EditorGUI.EndChangeCheck()) {
				minValueProperty.floatValue = newMinValue;
				maxValueProperty.floatValue = newMaxValue;
			}

		}

		private void ClampValuesToLimits(
			MinMaxRangeAttribute minMaxRangeAttribute,
			SerializedProperty minValueProperty, 
			SerializedProperty maxValueProperty
		) {
			if (minValueProperty.floatValue < minMaxRangeAttribute.MinLimit) {
				minValueProperty.floatValue = minMaxRangeAttribute.MinLimit;
			}
			if (minValueProperty.floatValue > minMaxRangeAttribute.MaxLimit) {
				minValueProperty.floatValue = minMaxRangeAttribute.MaxLimit;
			}
			if (maxValueProperty.floatValue < minMaxRangeAttribute.MinLimit) {
				maxValueProperty.floatValue = minMaxRangeAttribute.MinLimit;
			}
			if (maxValueProperty.floatValue > minMaxRangeAttribute.MaxLimit) {
				maxValueProperty.floatValue = minMaxRangeAttribute.MaxLimit;
			}
		}

		#endregion


	}
}