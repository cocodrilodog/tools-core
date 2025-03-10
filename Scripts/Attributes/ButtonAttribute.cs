namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Adds a inspector button that invokes the corresponding method. Supported on MonoBehaviour, 
	/// ScriptableObject and <see cref="CDObject"/> derived classes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ButtonAttribute : PropertyAttribute {


		#region Public Properties

		/// <summary>
		/// The index of the button in the inspector.
		/// </summary>
		public int Index => m_Index;

		// TODO: Implement this property
		public bool HorizontalizeSameIndex => m_HorizontalizeSameIndex;

		#endregion


		#region Constructor

		/// <summary>
		/// Adds a inspector button that invokes the corresponding method. Supported on MonoBehaviour, 
		/// ScriptableObject and <see cref="CDObject"/> derived classes.
		/// </summary>
		/// 
		/// <param name="index">The index of the button in the inspector.</param>
		public ButtonAttribute(int index = 0, bool horizontalizeSameIndex = false) {
			m_Index = index;
			m_HorizontalizeSameIndex = horizontalizeSameIndex;
		}

		#endregion


		#region Private Fields

		private int m_Index;

		private bool m_HorizontalizeSameIndex;

		#endregion


	}

}
