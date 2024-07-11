namespace CocodriloDog.Core {
	
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// An attribute for string fields that will show a popup with a list of tags that are defined 
	/// in a <see cref="StringOptions"/> asset.
	/// </summary>
	public class StringOptionsAttribute : PropertyAttribute {


		#region Public Constructors

		/// <summary>
		/// Creates the attribute.
		/// </summary>
		/// <param name="groupName">The name of the <see cref="StringOptions"/> asset.</param>
		public StringOptionsAttribute(string groupName) {
			m_GroupName = groupName;
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// The name of the group. It should be the same name of the <see cref="StringOptions"/> asset or
		/// the name of a field that references the asset.
		/// </summary>
		public string GroupName => m_GroupName;

		#endregion


		#region Private Fields

		private string m_GroupName;

		#endregion


	}

}
