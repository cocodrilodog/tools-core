namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Use this attribute to make a group of events with the same group name, to appear under 
	/// a tool bar in the inspector.
	/// </summary>
	public class UnityEventGroupAttribute : PropertyAttribute {


		#region Public Methods

		public string GroupName => m_GroupName;

		#endregion


		#region Constructors

		/// <summary>
		/// Creates the attribute.
		/// </summary>
		/// <param name="groupName">
		/// The name of the group. Multiple events that share the same name will appear under the same group.
		/// </param>
		public UnityEventGroupAttribute(string groupName) {
			m_GroupName = groupName;
		}

		#endregion


		#region Private Fields

		private string m_GroupName;

		#endregion


	}

}
