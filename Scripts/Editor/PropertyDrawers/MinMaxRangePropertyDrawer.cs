namespace CocodriloDog.Core {

	using UnityEngine;
	using UnityEditor;
	using System.Collections;
	using System.Collections.Generic;

	[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
	public class MinMaxRangePropertyDrawer : PropertyDrawerBase {


		#region Public Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			base.GetPropertyHeight(property, label);
			if (Property.type == typeof(FloatRange).Name || Property.type == typeof(IntRange).Name) {
				return FieldHeight * 1;
			} else {
				// Shows HelpBox
				return FieldHeight * 2;
			}
		}

		#endregion


		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);

			if (Property.type == typeof(FloatRange).Name || Property.type == typeof(IntRange).Name) {
				// This assignment is required for the tooltip to be shown
				Label = EditorGUI.BeginProperty(Position, Label, Property);
				EditorGUI.LabelField(GetNextPosition(), Label);
				DrawMinMaxControls();
				ClampValuesToLimits();
				EditorGUI.EndProperty();
			} else {
				EditorGUI.HelpBox(
					Position,
					string.Format(
						"{0} only supports {1} and {2}.",
						typeof(MinMaxRangeAttribute).Name,
						typeof(FloatRange).Name,
						typeof(IntRange).Name
					),
					MessageType.Error
				);
			}

		}

		#endregion


		#region Protected Methods

		protected override void InitializePropertiesForOnGUI() {
			base.InitializePropertiesForOnGUI();
			MinValueProperty = Property.FindPropertyRelative("m_MinValue");
			MaxValueProperty = Property.FindPropertyRelative("m_MaxValue");
			MinMaxRangeAttribute = attribute as MinMaxRangeAttribute;
		}

		#endregion


		#region Private Properties

		private SerializedProperty MinValueProperty { get; set; }

		private SerializedProperty MaxValueProperty { get; set; }

		private MinMaxRangeAttribute MinMaxRangeAttribute { get; set; }

		#endregion


		#region Private Methods

		private Rect GetMinMaxControlsRect() {
			Rect minMaxControlsRect = Position;
			minMaxControlsRect.x += EditorGUIUtility.labelWidth;
			minMaxControlsRect.width -= EditorGUIUtility.labelWidth;
			return minMaxControlsRect;
		}

		private void DrawMinMaxControls() {

			Rect position = GetMinMaxControlsRect();

			float valueFieldWidth = position.width * 0.2f;

			Rect minValueFieldPosition = position;
			minValueFieldPosition.width = valueFieldWidth - 4;
			DrawMinValue(minValueFieldPosition, MinValueProperty, MaxValueProperty);

			Rect maxValueFieldPosition = position;
			maxValueFieldPosition.x = position.xMax - valueFieldWidth + 4;
			maxValueFieldPosition.width = valueFieldWidth - 4;
			DrawMaxValue(maxValueFieldPosition, MinValueProperty, MaxValueProperty);

			Rect sliderPosition = position;
			sliderPosition.x += valueFieldWidth;
			sliderPosition.width -= valueFieldWidth * 2;
			DrawMinMaxSlider(sliderPosition, MinMaxRangeAttribute, MinValueProperty, MaxValueProperty);

		}

		private void DrawMinValue(
			Rect position, SerializedProperty minValueProperty, SerializedProperty maxValueProperty
		) {
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position, minValueProperty, GUIContent.none);
			if (EditorGUI.EndChangeCheck()) {
				if (Property.type == typeof(FloatRange).Name) {
					if (minValueProperty.floatValue > maxValueProperty.floatValue) {
						maxValueProperty.floatValue = minValueProperty.floatValue;
					}
				}
				if (Property.type == typeof(IntRange).Name) {
					if (minValueProperty.intValue > maxValueProperty.intValue) {
						maxValueProperty.intValue = minValueProperty.intValue;
					}
				}
			}
		}

		private void DrawMaxValue(
			Rect position, SerializedProperty minValueProperty, SerializedProperty maxValueProperty
		) {
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position, maxValueProperty, GUIContent.none);
			if (EditorGUI.EndChangeCheck()) {
				if (Property.type == typeof(FloatRange).Name) {
					if (maxValueProperty.floatValue < minValueProperty.floatValue) {
						minValueProperty.floatValue = maxValueProperty.floatValue;
					}
				}
				if (Property.type == typeof(IntRange).Name) {
					if (maxValueProperty.intValue < minValueProperty.intValue) {
						minValueProperty.intValue = maxValueProperty.intValue;
					}
				}
			}
		}

		private void DrawMinMaxSlider(
			Rect position, 
			MinMaxRangeAttribute minMaxRangeAttribute,
			SerializedProperty minValueProperty,
			SerializedProperty maxValueProperty
		) {
			if (Property.type == typeof(FloatRange).Name) {

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
			if (Property.type == typeof(IntRange).Name) {

				float newMinValue = minValueProperty.intValue;
				float newMaxValue = maxValueProperty.intValue;

				EditorGUI.BeginChangeCheck();
				// This only accept floats
				EditorGUI.MinMaxSlider(
					position,
					ref newMinValue, ref newMaxValue,
					minMaxRangeAttribute.MinLimit, minMaxRangeAttribute.MaxLimit
				);

				if (EditorGUI.EndChangeCheck()) {
					minValueProperty.intValue = (int)newMinValue;
					maxValueProperty.intValue = (int)newMaxValue;
				}

			}
		}

		private void ClampValuesToLimits() {
			if (Property.type == typeof(FloatRange).Name) {
				if (MinValueProperty.floatValue < MinMaxRangeAttribute.MinLimit) {
					MinValueProperty.floatValue = MinMaxRangeAttribute.MinLimit;
				}
				if (MinValueProperty.floatValue > MinMaxRangeAttribute.MaxLimit) {
					MinValueProperty.floatValue = MinMaxRangeAttribute.MaxLimit;
				}
				if (MaxValueProperty.floatValue < MinMaxRangeAttribute.MinLimit) {
					MaxValueProperty.floatValue = MinMaxRangeAttribute.MinLimit;
				}
				if (MaxValueProperty.floatValue > MinMaxRangeAttribute.MaxLimit) {
					MaxValueProperty.floatValue = MinMaxRangeAttribute.MaxLimit;
				}
			}
			if (Property.type == typeof(IntRange).Name) {
				if (MinValueProperty.intValue < MinMaxRangeAttribute.MinLimit) {
					MinValueProperty.intValue = (int)MinMaxRangeAttribute.MinLimit;
				}
				if (MinValueProperty.intValue > MinMaxRangeAttribute.MaxLimit) {
					MinValueProperty.intValue = (int)MinMaxRangeAttribute.MaxLimit;
				}
				if (MaxValueProperty.intValue < MinMaxRangeAttribute.MinLimit) {
					MaxValueProperty.intValue = (int)MinMaxRangeAttribute.MinLimit;
				}
				if (MaxValueProperty.intValue > MinMaxRangeAttribute.MaxLimit) {
					MaxValueProperty.intValue = (int)MinMaxRangeAttribute.MaxLimit;
				}
			}
		}

		#endregion


	}
}