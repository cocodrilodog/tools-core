namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ScriptableGetterBase), true)]
	public class ScriptableGetterPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);
			Label = EditorGUI.BeginProperty(Position, Label, Property);

			var useAssetAssetProperty = Property.FindPropertyRelative("m_UseAsset");
			var assetProperty = Property.FindPropertyRelative("m_Asset");
			var valueProperty = Property.FindPropertyRelative("m_Value");
			
			var fieldRect = GetNextPosition();
			fieldRect.xMax -= 22;

			var buttonRect = fieldRect;
			buttonRect.xMin = fieldRect.xMax + 2;
			buttonRect.width = 20;

			if (useAssetAssetProperty.boolValue) {
				EditorGUI.PropertyField(fieldRect, assetProperty, Label);
				if (GUI.Button(buttonRect, new GUIContent("A", "Switch to value mode"))) {
					useAssetAssetProperty.boolValue = false;
				}
			} else {
				EditorGUI.PropertyField(fieldRect, valueProperty, Label);
				if (GUI.Button(buttonRect, new GUIContent("V", "Switch to asset mode"))) {
					useAssetAssetProperty.boolValue = true;
				}
			}

			EditorGUI.EndProperty();

		}

		#endregion


	}

}