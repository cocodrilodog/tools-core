namespace CocodriloDog.Core {

	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyPropertyDrawer : PropertyDrawerBase {


		#region Public Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			base.GetPropertyHeight(property, label);
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);

			GUI.enabled = false;
			var type = CDEditorUtility.GetPropertyType(Property);
			if (SystemUtility.IsSubclassOfRawGeneric(type, typeof(ListWrapper<>))) {
				// Draw the ListWrapper property
				var listProperty = Property.FindPropertyRelative("m_List");
				EditorGUI.PropertyField(GetNextPosition(listProperty), listProperty, new GUIContent(Property.displayName, Property.tooltip));
			} else {
				// Draw the normal property
				EditorGUI.PropertyField(GetNextPosition(Property), Property);
			}
			GUI.enabled = true;

		}

		#endregion


	}

}