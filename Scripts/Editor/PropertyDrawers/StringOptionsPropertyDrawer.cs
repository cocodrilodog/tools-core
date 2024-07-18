namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Reflection;
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
					var index = StringOptions.IndexOf(property.stringValue) + 1;
					var options = new GUIContent[StringOptions.Count + 1];
					options[0] = new GUIContent("None");
					for(int i = 1; i < options.Length; i++) {
						options[i] = new GUIContent(StringOptions[i - 1]);
					}
					index = EditorGUI.Popup(rect, label, index, options);
					if(index > 0 && index < options.Length) {
						property.stringValue = StringOptions[index - 1];
					} else {
						// index = 0
						property.stringValue = null;
					}
				} else {
					EditorGUI.PropertyField(rect, Property, label);
				}
			} else {
				EditorGUI.HelpBox(rect, $"{nameof(StringOptionsAttribute)} is only valid for string properties", MessageType.Error);
			}

			EditorGUI.EndProperty();

		}

		#endregion


		#region Private Fields

		private StringOptions m_StringOptionsAsset;

		private MethodInfo m_StringOptionsMethod;

		#endregion


		#region Private Properties

		private ReadOnlyCollection<string> StringOptions {
			get {

				var tagGroupAttribute = attribute as StringOptionsAttribute;

				// Try to look for an asset
				if (m_StringOptionsAsset == null && m_StringOptionsMethod == null) { 
					m_StringOptionsAsset = Resources.Load<StringOptions>(tagGroupAttribute.OptionsName);
				}

				// Try to look for an asset field
				if (m_StringOptionsAsset == null && m_StringOptionsMethod == null) {
					m_StringOptionsAsset = Property.serializedObject.FindProperty(tagGroupAttribute.OptionsName)?.objectReferenceValue as StringOptions;
				}

				// Return the asset value
				if (m_StringOptionsAsset != null) {
					return m_StringOptionsAsset.Options;
				}

				// Try to look for a method
				if (m_StringOptionsAsset == null && m_StringOptionsMethod == null) {
					m_StringOptionsMethod = CDEditorUtility.GetMethod(Property.serializedObject.targetObject, tagGroupAttribute.OptionsName);
				}

				// Return the method value
				if (m_StringOptionsMethod != null) {
					var list = m_StringOptionsMethod.Invoke(Property.serializedObject.targetObject, null) as List<string>;
					if (list != null) {
						return new ReadOnlyCollection<string>(list);
					}
					var array = m_StringOptionsMethod.Invoke(Property.serializedObject.targetObject, null) as string[];
					if (array != null) {
						return new ReadOnlyCollection<string>(array);
					}
				}

				// Nothing left
				return null;

			}
		}

		#endregion


	}

}
