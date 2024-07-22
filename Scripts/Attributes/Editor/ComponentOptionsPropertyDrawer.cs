namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ComponentOptions))]
	public class ComponentOptionsPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);

			Label = EditorGUI.BeginProperty(Position, Label, Property);

			var gameObject = (Property.serializedObject.targetObject as MonoBehaviour).gameObject;
			var options = new List<Object> { gameObject };
			for(int i = 0; i < gameObject.GetComponentCount(); i++) {
				var component = gameObject.GetComponentAtIndex(i);
				var exludeTypes = new List<string>((attribute as ComponentOptions).ExludeTypes);
				if (!exludeTypes.Contains(component.GetType().Name)) {
					options.Add(component);
				}
			}

			var optionNames = new List<GUIContent>();
			foreach(var option in options) {
				optionNames.Add(new GUIContent(option.GetType().Name));
			}

			var index = options.IndexOf(Property.objectReferenceValue);
			index = index == -1 ? 0 : index;
			index = EditorGUI.Popup(GetNextPosition(), Label, index, optionNames.ToArray());
			Property.objectReferenceValue = options[index];

			EditorGUI.EndProperty();

		}

		#endregion


	}

}