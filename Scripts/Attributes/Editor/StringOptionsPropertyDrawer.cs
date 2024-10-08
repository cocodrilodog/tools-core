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

			var stringOptions = GetStringOptions();

			if (property.propertyType == SerializedPropertyType.String) {
				if(stringOptions != null) { 
					var index = stringOptions.IndexOf(property.stringValue) + 1;
					var options = new GUIContent[stringOptions.Count + 1];
					options[0] = new GUIContent("None");
					for(int i = 1; i < options.Length; i++) {
						options[i] = new GUIContent(stringOptions[i - 1]);
					}
					index = EditorGUI.Popup(rect, label, index, options);
					if(index > 0 && index < options.Length) {
						property.stringValue = stringOptions[index - 1];
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


		#region Private Properties

		private ReadOnlyCollection<string> GetStringOptions() {

			var tagGroupAttribute = attribute as StringOptionsAttribute;

			// Try to look for an asset in the resources folder
			var stringOptionsAsset = Resources.Load<StringOptions>(tagGroupAttribute.OptionsName);
			if (stringOptionsAsset != null) {
				return stringOptionsAsset.Options;
			}

			// Try to get the parent property, which will be used to look for asset or method
			var parentProperty = CDEditorUtility.GetNonArrayOrListParentProperty(Property);

			// If not found, try to look for an asset field
			if (stringOptionsAsset == null) {
				// Look in the parent property first to support system objects
				var assetProperty = parentProperty?.FindPropertyRelative(tagGroupAttribute.OptionsName);
				// If not, resort to the serialized object
				if (assetProperty == null) {
					assetProperty = Property.serializedObject.FindProperty(tagGroupAttribute.OptionsName);
				}
				stringOptionsAsset = assetProperty?.objectReferenceValue as StringOptions;
			}

			if (stringOptionsAsset != null) {
				return stringOptionsAsset.Options;
			}

			// If no asset has been found yet, try to look for a method
			object targetObject = null;
			MethodInfo stringOptionsMethod = null;

			// First look in the parent property to support system objects
			if (parentProperty != null) {
				targetObject = CDEditorUtility.GetPropertyValue(parentProperty);
				stringOptionsMethod = CDEditorUtility.GetMethod(targetObject, tagGroupAttribute.OptionsName);
			}

			// If that fails, resort to serializedObject.targetObject
			if (stringOptionsMethod == null) {
				targetObject = Property.serializedObject.targetObject;
				stringOptionsMethod = CDEditorUtility.GetMethod(targetObject, tagGroupAttribute.OptionsName);
			}

			// If a method was found, return the method value
			if (stringOptionsMethod != null) {
				var list = stringOptionsMethod.Invoke(targetObject, null) as List<string>;
				if (list != null) {
					return new ReadOnlyCollection<string>(list);
				}
				var array = stringOptionsMethod.Invoke(targetObject, null) as string[];
				if (array != null) {
					return new ReadOnlyCollection<string>(array);
				}
			}

			// Nothing left to do
			return null;

		}

		#endregion


	}

}
