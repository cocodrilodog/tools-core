namespace CocodriloDog.Core { //TODO: Move this to Utility

	using System;
	using System.Reflection;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UIElements;
	using UnityEditor;
	using UnityEditor.UIElements;


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

		#endregion


		#region Public Static Methods - Reflection

		/// <summary>
		/// Gets a MethodInfo by its name.
		/// </summary>
		/// <returns>The method.</returns>
		/// <param name="target">Target.</param>
		/// <param name="methodName">Method name.</param>
		public static MethodInfo GetMethod(UnityEngine.Object target, string methodName) {

			MethodInfo method = null;

			// Try to get the method from the current type
			Type type = target.GetType();
			method = type.GetMethod(
				methodName,
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
			);

			// If not found go ahead with the base classes
			if (method == null) {
				while (type.BaseType != null) {
					type = type.BaseType;
					method = type.GetMethod(
						methodName,
						BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
					);
					if (method != null) {
						break;
					}
				}
			}

			// If method with the provided name is not found
			if (method == null) {
				throw new InvalidOperationException(
					string.Format("Didn't find a method named \"{0}\"", methodName)
				);
			}

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