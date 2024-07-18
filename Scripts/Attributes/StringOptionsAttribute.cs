namespace CocodriloDog.Core {
	
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// An attribute for string fields that will show a popup with a list of strings that are defined 
	/// in a <see cref="StringOptions"/> asset or in a method.
	/// </summary>
	public class StringOptionsAttribute : PropertyAttribute {


		#region Public Constructors

		/// <summary>
		/// Creates the attribute.
		/// </summary>
		/// <param name="optionsName">
		/// The name of a <see cref="StringOptions"/> asset, a <see cref="StringOptions"/> field or a 
		/// method that returns a list of strings.
		/// </param>
		public StringOptionsAttribute(string optionsName) {
			m_OptionsName = optionsName;
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// The name of the group. It should be the same name of the <see cref="StringOptions"/> asset or
		/// the name of a field that references the asset.
		/// </summary>
		public string OptionsName => m_OptionsName;

		#endregion


		#region Private Fields

		private string m_OptionsName;

		#endregion


	}

}
