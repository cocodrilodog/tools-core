namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Property drawer for system objects that allows to draw child properties individually
	/// with <see cref="GetChildPropertyHeight(UnityEditor.SerializedProperty)"/>
	/// and <see cref="DrawChildProperty(UnityEditor.SerializedProperty)"/> and supports
	/// the <see cref="ButtonAttribute"/> on child properties.
	/// </summary>
	[CustomPropertyDrawer(typeof(CDObject), true)]
	public class CDObjectPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			
			base.GetPropertyHeight(property, label);

			// The foldout's height
			float height = EditorGUIUtility.singleLineHeight + 2;
			if (Property.isExpanded) {

				// The child properties height
				// Get the type to get the methods with button later
				if (m_Type == null) {
					m_Type = CDEditorUtility.GetPropertyType(Property);
				}

				// Iterate the properties
				var index = 0;
				CDEditorUtility.IterateChildProperties(Property, p => {

					// Add the property height
					height += GetChildPropertyHeight(p);

					// Add the height of the buttons of the methods with button
					if (s_MethodsWithButtonByType.TryGetValue(m_Type, out var methodsWithButtonByIndex)) {
						if (methodsWithButtonByIndex.TryGetValue(index, out var methods)) {
							foreach (var method in methods) {
								height += EditorGUIUtility.singleLineHeight + 2;
							}
						}
					}

					index++;

				});

			}

			return height;

		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			
			base.OnGUI(position, property, label);

			// Collect the methods with button attribute
			CDEditorUtility.GetPropertyValueAndType(Property, out var value, out var m_Type);
			if(s_MethodsWithButtonByType.TryAdd(m_Type, new Dictionary<int, List<MethodInfo>>())) {

				// Store methods that have ButtonAttribute here
				var methodsWithButtonByIndex = new Dictionary<int, List<MethodInfo>>();

				// Get all methods of the target object
				MethodInfo[] allMethods = m_Type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

				// If the method has the ButtonAttribute, store it
				foreach (MethodInfo method in allMethods) {
					var buttonAttribute = System.Attribute.GetCustomAttribute(method, typeof(ButtonAttribute)) as ButtonAttribute;
					if (buttonAttribute != null) {
						// The attribute may be used more than once with the same index, so we store
						// the methods in a list
						methodsWithButtonByIndex.TryAdd(buttonAttribute.Index, new List<MethodInfo>());
						methodsWithButtonByIndex[buttonAttribute.Index].Add(method);
					}
				}

				s_MethodsWithButtonByType[m_Type] = methodsWithButtonByIndex; 

			}

			// Draw the object property with its children
			Property.isExpanded = EditorGUI.PropertyField(GetNextPosition(1), Property, label, false);
			EditorGUI.indentLevel++;
			if (Property.isExpanded) {
				var index = 0;
				CDEditorUtility.IterateChildProperties(Property, p => {
					DrawChildProperty(p);
					var methodsWithButtonByIndex = s_MethodsWithButtonByType[m_Type];
					if (methodsWithButtonByIndex.TryGetValue(index, out var methods)) {
						foreach (var method in methods) {
							var buttonRect = GetNextPosition(1);
							buttonRect.xMin += EditorGUI.indentLevel * 15;
							if (GUI.Button(buttonRect, ObjectNames.NicifyVariableName(method.Name))) {
								method.Invoke(value, null);
							}
						}
					}
					index++;
				});
			}
			EditorGUI.indentLevel--;

		}

		#endregion


		#region Protected Methods

		protected virtual float GetChildPropertyHeight(SerializedProperty property) {
			return EditorGUI.GetPropertyHeight(property, true) + 2;
		}

		protected virtual void DrawChildProperty(SerializedProperty property) {
			EditorGUI.PropertyField(GetNextPosition(property), property, true);
		}

		#endregion


		#region Private Static Fields

		private static Dictionary<Type, Dictionary<int, List<MethodInfo>>> s_MethodsWithButtonByType =
			new Dictionary<Type, Dictionary<int, List<MethodInfo>>>();

		#endregion


		#region Private Fields

		private Type m_Type;

		#endregion


	}

}