namespace CocodriloDog.Core {

	using System;
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

		public static void DelayedAction(Action action, float delay) {
			DelayedActionInfo.Action = action;
			DelayedActionInfo.Delay = delay;
			DelayedActionInfo.Time = EditorApplication.timeSinceStartup;
			EditorApplication.update += EditorApplication_Update_DelayedAction;
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