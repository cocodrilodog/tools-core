namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using UnityEngine;

	/// <summary>
	/// An <see cref="ScriptableObject"/> with a list of strings to be used as options by a string field when 
	/// the <see cref="StringOptionsAttribute"/> is in the field. It will be displayed as a popup with the options
	/// in the inspector.
	/// </summary>
	[CreateAssetMenu(menuName = "Cocodrilo Dog/Core/String Options")]
	public class StringOptions : ScriptableObject {


		#region Public Properties

		public ReadOnlyCollection<string> Options => m_OptionsRO = m_OptionsRO ?? new ReadOnlyCollection<string>(m_Options);

		#endregion


		#region Private Fields - Serialized

		[SerializeField]
		private List<string> m_Options;

		[SerializeField]
		private DocumentationComment m_DocumentationComment;

		#endregion


		#region Private Fields - Non Serialized

		[NonSerialized]
		private ReadOnlyCollection<string> m_OptionsRO;

		#endregion


	}

}
