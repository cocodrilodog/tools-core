namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ScriptableFieldBase), true)]
	public class ScriptableFieldPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

			var defaultHeight = base.GetPropertyHeight(property, label);

			if (m_UseAssetAssetProperty.boolValue) {
				return defaultHeight;
			} else {
				return EditorGUI.GetPropertyHeight(m_ValueProperty);
			}

		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);
			Label = EditorGUI.BeginProperty(Position, Label, Property);
			
			var fieldRect = GetNextPosition();
			fieldRect.xMax -= 22;

			var buttonRect = fieldRect;
			buttonRect.xMin = fieldRect.xMax + 2;
			buttonRect.width = 20;

			// Modify this for the icon to look good
			var buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.padding = new RectOffset(1, 1, 1, 1);
			var iconSize = EditorGUIUtility.GetIconSize();
			EditorGUIUtility.SetIconSize(new Vector2(12, 12));

			if (m_UseAssetAssetProperty.boolValue) {

				EditorGUI.PropertyField(fieldRect, m_AssetProperty, Label);

				//// Using the system icon
				//var assetIcon = EditorGUIUtility.IconContent("ScriptableObject Icon");
				//assetIcon.tooltip = "Turn off asset mode";
				
				// This texture looks better
				var texture = Resources.Load($"AssetModeOnIcon") as Texture;
				var assetIcon = new GUIContent(texture, "Turn off asset mode");

				if (GUI.Button(buttonRect, assetIcon, buttonStyle)) {
					m_UseAssetAssetProperty.boolValue = false;
				}

			} else {

				EditorGUI.PropertyField(fieldRect, m_ValueProperty, Label, true);

				var texture = Resources.Load($"AssetModeOffIcon") as Texture;
				var assetIcon = new GUIContent(texture, "Turn on asset mode");

				if (GUI.Button(buttonRect, assetIcon, buttonStyle)) {
					m_UseAssetAssetProperty.boolValue = true;
				}

			}

			EditorGUIUtility.SetIconSize(iconSize);

			EditorGUI.EndProperty();

		}

		#endregion


		#region Protected Methods

		protected override void InitializePropertiesForGetHeight() {
			base.InitializePropertiesForGetHeight();
			m_UseAssetAssetProperty = Property.FindPropertyRelative("m_UseAsset");
			m_ValueProperty = Property.FindPropertyRelative("m_Value");
		}

		protected override void InitializePropertiesForOnGUI() {
			base.InitializePropertiesForOnGUI();
			m_UseAssetAssetProperty = Property.FindPropertyRelative("m_UseAsset");
			m_AssetProperty = Property.FindPropertyRelative("m_Asset");
			m_ValueProperty = Property.FindPropertyRelative("m_Value");
		}

		#endregion


		#region Private Fields

		private SerializedProperty m_UseAssetAssetProperty;

		private SerializedProperty m_AssetProperty;

		private SerializedProperty m_ValueProperty;

		#endregion


	}

}