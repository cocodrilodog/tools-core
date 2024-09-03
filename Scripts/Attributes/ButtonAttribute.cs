namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Adds a inspector button that invokes the corresponding method.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ButtonAttribute : PropertyAttribute {


		#region Public Properties

		/// <summary>
		/// The index of the button in the inspector.
		/// </summary>
		public int Index => m_Index;

		#endregion


		#region Constructor

		/// <summary>
		/// Adds a inspector button that invokes the corresponding method.
		/// </summary>
		/// <param name="index">The index of the button in the inspector.</param>
		public ButtonAttribute(int index = 0) {
			m_Index = index;
		}

		#endregion


		#region Private Fields

		private int m_Index;

		#endregion


	}

}
