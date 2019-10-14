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