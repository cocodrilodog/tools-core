namespace CocodriloDog.Core.Examples {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A file that contains text data.
	/// </summary>
	[Serializable]
	public class TextFile : FileBase {


		#region Private Fields

		[TextArea(5, 15)]
		[SerializeField]
		private string m_Text;

		#endregion


		#region Private Methods

		[Button]
		private void LogContent() {
			Debug.Log(m_Text);
		}

		#endregion


	}

}