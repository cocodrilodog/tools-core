namespace CocodriloDog.Core {

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


	public static class CDEditorUtility {


		#region Public Static Methods

		/// <summary>
		/// Performs an <paramref name="action"/> after the <paramref name="delay"/>
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="delay">Delay.</param>
		public static void DelayedAction(Action action, float delay) {
			DelayedActionInfo.Action = action;
			DelayedActionInfo.Delay = delay;
			DelayedActionInfo.Time = EditorApplication.timeSinceStartup;
			EditorApplication.update -= EditorApplication_Update_DelayedAction; // <- Prevent multiple adds of the event handler
			EditorApplication.update += EditorApplication_Update_DelayedAction;
		}

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


		#region Event Handlers

		private static void EditorApplication_Update_DelayedAction() {
			double elapsed = EditorApplication.timeSinceStartup - DelayedActionInfo.Time;
			if (elapsed >= DelayedActionInfo.Delay) {
				EditorApplication.update -= EditorApplication_Update_DelayedAction;
				DelayedActionInfo.Action();
				m_DelayedActionInfo = null;
			}
		}

		#endregion


		#region Private Static Fields

		private static DelayedActionInfo m_DelayedActionInfo;

		#endregion


		#region Private Properties

		private static DelayedActionInfo DelayedActionInfo {
			get { return m_DelayedActionInfo = m_DelayedActionInfo ?? new DelayedActionInfo(); }
		}

		#endregion


	}

}