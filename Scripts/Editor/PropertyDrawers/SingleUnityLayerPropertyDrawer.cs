namespace CocodriloDog.Core {

	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(SingleUnityLayer))]
	public class SingleUnityLayerPropertyDrawer : PropertyDrawer {


		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			EditorGUI.BeginProperty(position, GUIContent.none, property);
			SerializedProperty layerIndex = property.FindPropertyRelative("m_LayerIndex");

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			if (layerIndex != null) {
				layerIndex.intValue = EditorGUI.LayerField(position, layerIndex.intValue);
			}

			EditorGUI.EndProperty();

		}

		#endregion


	}

}