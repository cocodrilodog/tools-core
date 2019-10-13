namespace CocodriloDog.Core {

	using System;
	using System.Reflection;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;


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


	public static class EditorUtils {


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
			EditorApplication.update += EditorApplication_Update_DelayedAction;
		}

		/// <summary>
		/// Used by attribute property drawers to validate the received field.
		/// </summary>
		/// <returns><c>true</c>, if field drawable was ised, <c>false</c> otherwise.</returns>
		/// <param name="fieldInfo">The field info of the property drawer.</param>
		/// <param name="requiredType">The type that is required by the property drawer.</param>
		public static bool IsFieldDrawable(FieldInfo fieldInfo, Type requiredType) {

			Type fieldType = fieldInfo.FieldType;

			if (fieldInfo.FieldType.IsArray) {
				// Is an array
				fieldType = fieldInfo.FieldType.GetElementType();
			} else if (fieldInfo.FieldType.IsGenericType) {
				if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType)) {
					// Is a list
					fieldType = fieldInfo.FieldType.GenericTypeArguments[0];
				}
			}

			if (requiredType.IsAssignableFrom(fieldType)) {
				return true;
			}

			return false;
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