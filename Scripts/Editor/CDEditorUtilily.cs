namespace CocodriloDog.Core {

	using System;
	using System.Reflection;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UIElements;
	using UnityEditor;
	using UnityEditor.UIElements;
	using System.Linq;
	using System.Text.RegularExpressions;


	#region Small Types

	public class DelayedActionInfo {

		[SerializeField]
		public Action Action;

		[SerializeField]
		public double Delay;

		[SerializeField]
		public double Time;

	}

	#endregion

	/// <summary>
	/// Utility class for editor-related tasks.
	/// </summary>
	[InitializeOnLoad]
	public static class CDEditorUtility {


		#region Static Constructors

		static CDEditorUtility() {
			EditorApplication.update += EditorApplication_Update;
		}

		#endregion


		#region Public Static Methods - Timers

		/// <summary>
		/// Performs an <paramref name="action"/> after the <paramref name="delay"/>
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="delay">Delay.</param>
		public static void DelayedAction(Action action, float delay) {
			var actionInfo = new DelayedActionInfo();
			actionInfo.Action = action;
			actionInfo.Delay = delay;
			actionInfo.Time = EditorApplication.timeSinceStartup;
			DelayedActionInfos.Add(actionInfo);
		}

		#endregion


		#region Public Static Methods - Drawing

		/// <summary>
		/// This is very often used to draw disabled script fields as standard Unity style.
		/// </summary>
		/// <param name="serializedProperty">The property</param>
		public static void DrawDisabledField(SerializedProperty serializedProperty) {
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField(serializedProperty);
			EditorGUI.EndDisabledGroup();
		}

		/// <summary>
		/// Gets a rect that is a horizontal fraction of the provided rect.
		/// </summary>
		/// <param name="rect">The original rect</param>
		/// <param name="x">
		/// The current x to place one rect after the other. It is updated at the end of the method
		/// </param>
		/// <param name="percent">The desired percent of width with respect to the original rect.</param>
		/// <param name="gap">How much gap between the rects.</param>
		/// <returns></returns>
		public static Rect HorizontalFractionOfRect(Rect rect, ref float x, float percent, float gap = 10) {
			rect.x = x + gap * 0.5f;
			rect.width = rect.width * percent - gap;
			x += rect.width + gap;
			return rect;
		}

		/// <summary>
		/// Draws a horizontal line for inspectors.
		/// </summary>
		/// <param name="rect">The rectangle of the line</param>
		public static void DrawHorizontalLine(Rect rect) {
			var color = GUI.color;
			GUI.color = new Color(0.15f, 0.15f, 0.15f);
			GUI.Box(rect, GUIContent.none, HorizontalLineStyle);
			GUI.color = color;
		}
		
		/// <summary>
		/// Draws a horizontal line for inspectors with <see cref="GUILayout"/>.
		/// </summary>
		public static void DrawHorizontalLine() {
			var color = GUI.color;
			GUI.color = new Color(0.15f, 0.15f, 0.15f);
			GUILayout.Box(GUIContent.none, HorizontalLineStyle);
			GUI.color = color;
		}

		/// <summary>
		/// Creates a <see cref="IMGUIContainer"/> for the script property so that it works as the Unity standard
		/// way.
		/// </summary>
		/// <param name="serializedObject"></param>
		/// <returns></returns>
		public static IMGUIContainer GetScriptIMGUIContainer(SerializedObject serializedObject) {
			// Script. Using IMGUI version to allow click/double-click for highlight/select script
			SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");
			return new IMGUIContainer(() => {
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.PropertyField(scriptProperty);
				EditorGUI.EndDisabledGroup();
			});
		}

		#endregion


		#region Public Static Methods - Serialized Data

		/// <summary>
		/// If the provided <paramref name="property"/> is an array element, this assigns the index of 
		/// the element to <paramref name="index"/> and returns <c>true</c>, otherwise returns <c>false</c>
		/// and assigns -1 to <paramref name="index"/>.
		/// </summary>
		/// <param name="property">The <c>SerializedProperty</c></param>
		/// <param name="index">The index of the element</param>
		/// <returns><c>true</c> if the property is an array element, <c>false</c> otherwise.</returns>
		public static bool GetElementIndex(SerializedProperty property, out int index) {
			index = -1;
			if (IsArrayElement(property)) {
				index = GetElementIndex(property);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Determines whether the <paramref name="property"/> is an array element or not.
		/// </summary>
		/// <param name="property">The <c>SerializedProperty</c></param>
		/// <returns><c>true</c> if the property is an array element, <c>false</c> otherwise.</returns>
		public static bool IsArrayElement(SerializedProperty property) {
			return property.propertyPath.EndsWith(']');
		}

		/// <summary>
		/// If the provided <paramref name="property"/> is an array element, this gets the index of 
		/// the element.
		/// </summary>
		/// <remarks>
		/// <see cref="IsArrayElement(SerializedProperty)"/> should be used before to validate if the 
		/// <paramref name="property"/> is an array element or not.
		/// </remarks>
		/// <param name="property">The <c>SerializedProperty</c></param>
		/// <returns>The index of the array element.</returns>
		public static int GetElementIndex(SerializedProperty property) {
			var pathParts = property.propertyPath.Split('.');
			var indexString = string.Empty;
			// Parse only the last part which should be something like "data[0]"
			foreach (var c in pathParts[pathParts.Length - 1]) {
				if (Char.IsDigit(c)) {
					indexString += c;
				}
			}
			int index = int.Parse(indexString);
			return index;
		}

		/// <summary>
		/// Gets the value of the provided <paramref name="property"/>.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>The value</returns>
		public static object GetPropertyValue(SerializedProperty property) {
			GetPropertyValueAndType(property, out var value, out var _);
			return value;
		}

		/// <summary>
		/// Gets the type of the provided <paramref name="property"/>.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>The type</returns>
		public static Type GetPropertyType(SerializedProperty property) {
			switch (property.propertyType) {

				case SerializedPropertyType.AnimationCurve:	return typeof(AnimationCurve);
				case SerializedPropertyType.Boolean:		return typeof(bool);
				case SerializedPropertyType.Bounds:			return typeof(Bounds);
				case SerializedPropertyType.BoundsInt:		return typeof(Bounds);
				//case SerializedPropertyType.Character:				return ???;
				case SerializedPropertyType.Color:			return typeof(Color);
				//case SerializedPropertyType.Enum:						return ???;
				//case SerializedPropertyType.ExposedReference:			return ???;
				//case SerializedPropertyType.FixedBufferSize:			return ???;
				case SerializedPropertyType.Float:			return typeof(float);
				case SerializedPropertyType.Gradient:		return typeof(Gradient);
				case SerializedPropertyType.Hash128:		return typeof(Hash128);
				case SerializedPropertyType.Integer:		return typeof(int);
				case SerializedPropertyType.LayerMask:		return typeof(LayerMask);
				case SerializedPropertyType.Quaternion:		return typeof(Quaternion);
				case SerializedPropertyType.Rect:			return typeof(Rect);
				case SerializedPropertyType.RectInt:		return typeof(Rect);
				case SerializedPropertyType.String:			return typeof(string);
				case SerializedPropertyType.Vector2:		return typeof(Vector2);
				case SerializedPropertyType.Vector2Int:		return typeof(Vector2);
				case SerializedPropertyType.Vector3:		return typeof(Vector3);
				case SerializedPropertyType.Vector3Int:		return typeof(Vector3);
				case SerializedPropertyType.Vector4:		return typeof(Vector4);

				// ObjectReference is better handled in the generic way below. Otherwise when the value is null
				// the result would lose presicion. Commenting this for future reference
				//
				// case SerializedPropertyType.ObjectReference: 
				//	 return property.objectReferenceValue != null ? property.objectReferenceValue.GetType() : typeof(UnityEngine.Object);

				case SerializedPropertyType.ManagedReference: {
						// Example of managedReferenceFieldTypename: "CocodriloDog.Core.Examples CocodriloDog.Core.Examples.Folder"
						var typenameParts = property.managedReferenceFieldTypename.Split(' ');
						var assembly = Assembly.Load(typenameParts[0]);
						return assembly.GetType(typenameParts[1]);
					}

				default: {
						// For all types that are not included before, this is a generic way of getting the type
						// via propertyPath with Reflection
						GetPropertyValueAndType(property, out var _, out var type);
						return type;
					}

			}
		}

		/// <summary>
		/// Gets the <paramref name="value"/> and <paramref name="type"/> of the given <paramref name="property"/>.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="value">The value.</param>
		/// <param name="type">The type.</param>
		public static void GetPropertyValueAndType(SerializedProperty property, out object value, out Type type) {

			// Start with the target object
			FieldInfo field = null;
			object currentObject = property.serializedObject.targetObject;

			// Example of a path: m_MyDrive.m_Files.Array.data[2].m_Files
			// Example of a path: m_MyDrive.m_Files2.m_List
			var pathSteps = property.propertyPath.Split('.');

			for (int i = 0; i < pathSteps.Length; i++) {

				var step = pathSteps[i];

				// Array treatment
				if (step == "Array" && pathSteps.Length > i + 1 && pathSteps[i + 1].Contains("data[")) {

					var pattern = @"data\[(\d+)\]";
					var regex = new Regex(pattern);
					var match = regex.Match(pathSteps[i + 1]);

					if (match.Success) {
						var intString = match.Groups[1].Value;
						if (int.TryParse(intString, out int result)) {
							currentObject = (currentObject as IList)[result];
							i++;
							continue;
						}
					}

				}

				// Get the field that corresponds to the step
				var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
				var t = currentObject.GetType();
				field = t.GetField(step, bindingFlags);
				while (field == null && t != null) {
					// If not found, continue with super classes
					t = t.BaseType;
					field = t?.GetField(step, bindingFlags);
				}

				currentObject = field?.GetValue(currentObject);

			}

			value = currentObject;
			type = field?.FieldType;

		}

		/// <summary>
		/// Gets the parent property of the provided <paramref name="property"/> or <c>null</c> if it has no parent
		/// </summary>
		/// <param name="property">The property</param>
		/// <returns>The parent property</returns>
		public static SerializedProperty GetParentProperty(SerializedProperty property) {
			var lastDotIndex = property.propertyPath.LastIndexOf('.');
			if (lastDotIndex >= 0) {
				string parentPath = property.propertyPath.Remove(lastDotIndex);
				SerializedProperty parentProperty = property.serializedObject.FindProperty(parentPath);
				return parentProperty;
			}
			return null;
		}

		/// <summary>
		/// Gets the parent property of the provided <paramref name="property"/>, skipping arrays or lists
		/// or <c>null</c> if it has no parent
		/// </summary>
		/// <param name="property">The property</param>
		/// <returns>The parent property</returns>
		public static SerializedProperty GetNonArrayOrListParentProperty(SerializedProperty property) {
			var parentProperty = GetParentProperty(property);
			if (parentProperty != null) {
				if (IsArrayElement(property)) {
					// Move two more steps up on a path like this one:
					// "SomeObject.SomeList.Array"
					// ".data[0]" is already discarded by the first step up
					parentProperty = GetParentProperty(parentProperty);
					parentProperty = GetParentProperty(parentProperty);
				}
			}
			return parentProperty;
		}


		/// <summary>
		/// Iterates through the direct child properties of the <paramref name="serializedObject"/>, not grandchildren, etc.
		/// </summary>
		/// <param name="serializedObject">The <see cref="SerializedObject"/></param>
		/// <param name="action">The action to perform on each child property</param>
		public static void IterateChildProperties(SerializedObject serializedObject, Action<SerializedProperty> action) {

			// Get the iterator
			// Copy to leave the original unchanged
			var iterator = serializedObject.GetIterator().Copy();

			// Enter children needs to be true the first time
			bool enterChildren = true;

			// Iterate the child properties
			while (iterator.NextVisible(enterChildren)) {
				enterChildren = false;
				action(iterator);
			}

		}

		/// <summary>
		/// Iterates through the direct child properties of the <paramref name="parentProperty"/>, not grandchildren, etc.
		/// </summary>
		/// <param name="parentProperty">The parent <see cref="SerializedProperty"/></param>
		/// <param name="action">The action to perform on each child property</param>
		public static void IterateChildProperties(SerializedProperty parentProperty, Action<SerializedProperty> action) {

			// Copy to leave the original unchanged
			var parentCopy = parentProperty.Copy();

			// Get the first property that is not child of the parent property
			// This must be done before iterating the parent property
			var endProperty = parentCopy.GetEndProperty();

			// Get the first child as the iterator
			SerializedProperty iterator = null;
			foreach (SerializedProperty childProperty in parentCopy) {
				iterator = childProperty;
				break;
			}

			// Iterate the child properties
			action(iterator);
			while (iterator.NextVisible(false)) {
				if (SerializedProperty.EqualContents(iterator, endProperty)) {
					break;
				}
				action(iterator);
			}

		}

		#endregion


		#region Public Static Methods - Reflection

		/// <summary>
		/// Gets a MethodInfo by its name.
		/// </summary>
		/// <returns>The method.</returns>
		/// <param name="target">Target.</param>
		/// <param name="methodName">Method name.</param>
		public static MethodInfo GetMethod(object target, string methodName) {

			MethodInfo method;
			var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			// Try to get the method from the current type
			Type type = target.GetType();
			method = type.GetMethod(methodName, flags);

			// If not found go ahead with the base classes
			while (method == null) {
				type = type.BaseType;
				if (type == null) {
					break;
				}
				method = type.GetMethod(methodName, flags);
			}

			// TODO: This check was removed. This must be tested in all tools.
			//// If method with the provided name is not found
			//if (method == null) {
			//	throw new InvalidOperationException($"Didn't find a method named \"{methodName}\"");
			//}

			return method;

		}

		#endregion


		#region Event Handlers

		private static void EditorApplication_Update() {
			List<DelayedActionInfo> actionsToRemove = null;
			foreach(var actionInfo in DelayedActionInfos) {
				double elapsed = EditorApplication.timeSinceStartup - actionInfo.Time;
				if (elapsed >= actionInfo.Delay) {
					actionInfo.Action();
					actionsToRemove = actionsToRemove ?? new List<DelayedActionInfo>();
					actionsToRemove.Add(actionInfo);
				}
			}
			if (actionsToRemove != null) {
				foreach (var actionInfo in actionsToRemove) {
					DelayedActionInfos.Remove(actionInfo);
				}
			}
		}

		#endregion


		#region Private Static Fields

		private static List<DelayedActionInfo> m_DelayedActionInfos;

		private static GUIStyle m_HorizontalLineStyle;

		#endregion


		#region Private Properties

		private static List<DelayedActionInfo> DelayedActionInfos => 
			m_DelayedActionInfos = m_DelayedActionInfos ?? new List<DelayedActionInfo>();

		private static GUIStyle HorizontalLineStyle {
			get {
				if (m_HorizontalLineStyle == null) {
					m_HorizontalLineStyle = new GUIStyle();
					m_HorizontalLineStyle.normal.background = EditorGUIUtility.whiteTexture;
					m_HorizontalLineStyle.margin = new RectOffset(0, 0, 4, 4);
					m_HorizontalLineStyle.fixedHeight = 1;
				}
				return m_HorizontalLineStyle;
			}
		}

		#endregion


	}

}