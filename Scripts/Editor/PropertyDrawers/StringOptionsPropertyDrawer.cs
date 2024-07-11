namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Property drawer for the <see cref="StringOptionsAttribute"/>.
	/// </summary>
	[CustomPropertyDrawer(typeof(StringOptionsAttribute))]
	public class StringOptionsPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			
			base.OnGUI(position, property, label);

			label = EditorGUI.BeginProperty(position, label, property);
			var rect = GetNextPosition(property);
			
			if(property.propertyType == SerializedPropertyType.String) {
				if(StringOptions != null) { 
					var index = StringOptions.Options.IndexOf(property.stringValue) + 1;
					var options = new GUIContent[StringOptions.Options.Count + 1];
					options[0] = new GUIContent("None");
					for(int i = 1; i < options.Length; i++) {
						options[i] = new GUIContent(StringOptions.Options[i - 1]);
					}
					index = EditorGUI.Popup(rect, label, index, options);
					if(index > 0 && index < options.Length) {
						property.stringValue = StringOptions.Options[index - 1];
					} else {
						// index = 0
						property.stringValue = null;
					}
				} else {
					EditorGUI.HelpBox(rect, $"No TagGroup with name \"{(attribute as StringOptionsAttribute).GroupName}\" was found.", MessageType.Error);
				}
			} else {
				EditorGUI.HelpBox(rect, $"{nameof(StringOptionsAttribute)} is only valid for string properties", MessageType.Error);
			}

			EditorGUI.EndProperty();

		}

		#endregion


		#region Private Fields

		private StringOptions m_StringOptions;

		#endregion


		#region Private Properties

		private StringOptions StringOptions {
			get {
				// Try to look for an asset
				if (m_StringOptions == null) { 
					var tagGroupAttribute = attribute as StringOptionsAttribute;
					m_StringOptions = Resources.Load<StringOptions>(tagGroupAttribute.GroupName);
				}
				// Try to look for a property instead
				if (m_StringOptions == null) {
					var tagGroupAttribute = attribute as StringOptionsAttribute;
					m_StringOptions = Property.serializedObject.FindProperty(tagGroupAttribute.GroupName)?.objectReferenceValue as StringOptions;
				}
				return m_StringOptions;
			}
		}

		#endregion


	}

}
