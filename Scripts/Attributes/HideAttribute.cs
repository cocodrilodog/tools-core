namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Hides the property, depending on the results of the method implemented with <see cref="MethodName"/>
	/// </summary>
	public class HideAttribute : PropertyAttribute {


		#region Public Properties

		public string MethodName => m_MethodName;

		public int IndentDelta => m_IndentDelta;

		#endregion


		#region Public Constructor

		/// <summary>
		/// Constructor of the <see cref="HelpAttribute"/>
		/// </summary>
		/// <param name="methodName">
		/// The name of a method that must have this signature: <c>bool SomeMethod()</c>. If the method
		/// return <c>true</c>, the field will hide, otherwise, not.
		/// </param>
		/// <param name="indentDelta">Indent level for the field relative to the current indent level.</param>
		public HideAttribute(string methodName, int indentDelta = 0) {
			m_MethodName = methodName;
			m_IndentDelta = indentDelta;
		}

		#endregion


		#region Private Fields

		private string m_MethodName;

		private int m_IndentDelta;

		#endregion


	}

}