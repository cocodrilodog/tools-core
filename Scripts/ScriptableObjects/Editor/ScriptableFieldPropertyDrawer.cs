namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ScriptableFieldBase), true)]
	public class ScriptableFieldPropertyDrawer : PropertyDrawerBase {


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

			// Modify this for the icon to look good
			var buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.padding = new RectOffset(1, 1, 1, 1);
			var iconSize = EditorGUIUtility.GetIconSize();
			EditorGUIUtility.SetIconSize(new Vector2(12, 12));

			if (useAssetAssetProperty.boolValue) {

				EditorGUI.PropertyField(fieldRect, assetProperty, Label);

				//// Using the system icon
				//var assetIcon = EditorGUIUtility.IconContent("ScriptableObject Icon");
				//assetIcon.tooltip = "Turn off asset mode";
				
				// This texture looks better
				var texture = Resources.Load($"AssetModeOnIcon") as Texture;
				var assetIcon = new GUIContent(texture, "Turn off asset mode");

				if (GUI.Button(buttonRect, assetIcon, buttonStyle)) {
					useAssetAssetProperty.boolValue = false;
				}

			} else {

				EditorGUI.PropertyField(fieldRect, valueProperty, Label);

				var texture = Resources.Load($"AssetModeOffIcon") as Texture;
				var assetIcon = new GUIContent(texture, "Turn on asset mode");

				if (GUI.Button(buttonRect, assetIcon, buttonStyle)) {
					useAssetAssetProperty.boolValue = true;
				}

			}

			EditorGUIUtility.SetIconSize(iconSize);

			EditorGUI.EndProperty();

		}

		#endregion


	}

}