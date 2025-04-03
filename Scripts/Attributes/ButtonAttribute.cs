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

		/// <summary>
		/// If the buttons have the same index, they will be in horizontal layout when this is true.
		/// </summary>
		public bool HorizontalizeSameIndex => m_HorizontalizeSameIndex;

		/// <summary>
		/// Disables the button in edit mode.
		/// </summary>
		public bool DisableInEditMode => m_DisableInEditMode;

		/// <summary>
		/// Disables the button in play mode.
		/// </summary>
		public bool DisableInPlayMode => m_DisableInPlayMode;

		#endregion


		#region Constructor

		/// <summary>
		/// Adds a inspector button that invokes the corresponding method. Supported on MonoBehaviour, 
		/// ScriptableObject and <see cref="CDObject"/> derived classes.
		/// </summary>
		/// 
		/// <param name="index">The index of the button in the inspector.</param>
		/// 
		/// <param name="horizontalizeSameIndex">
		/// If the buttons have the same index, they will be in horizontal layout when this is true.
		/// </param>
		/// 
		/// <param name="disableInEditMode">Disables the button in edit mode.</param>
		/// <param name="disableInPlayMode">Disables the button in play mode.</param>
		public ButtonAttribute(
			int index = 0, 
			bool horizontalizeSameIndex = false, 
			bool disableInEditMode = false, 
			bool disableInPlayMode = false
		) {
			m_Index = index;
			m_HorizontalizeSameIndex = horizontalizeSameIndex;
			m_DisableInEditMode = disableInEditMode;
			m_DisableInPlayMode = disableInPlayMode;
		}

		#endregion


		#region Private Fields

		private int m_Index;

		private bool m_HorizontalizeSameIndex;

		private bool m_DisableInEditMode;

		private bool m_DisableInPlayMode;

		#endregion


	}

}
