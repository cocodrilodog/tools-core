namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Displays a help box under the property, depending on the results of the method
	/// implemented with <see cref="MethodName"/>
	/// </summary>
	public class HelpAttribute : PropertyAttribute {


		#region Public Properties

		public string MethodName => m_MethodName;

		#endregion


		#region Public Constructor

		/// <summary>
		/// Constructor of the <see cref="HelpAttribute"/>
		/// </summary>
		/// <param name="methodName">
		/// The name of a method that must have this signature: <c>int SomeMethod(ref string message)</c>
		/// And it can return 0 for no helpbox, 1 for info helpbox, 2 for warning helpbox, and 3 for error helpbox.
		/// </param>
		public HelpAttribute(string methodName) {
			m_MethodName = methodName;
		}

		#endregion


		#region Private Fields

		private string m_MethodName;

		#endregion


	}

}
