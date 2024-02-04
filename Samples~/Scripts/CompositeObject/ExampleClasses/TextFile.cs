namespace CocodriloDog.Core.Examples {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[Serializable]
	public class TextFile : FileBase {


		#region Private Fields

		[TextArea(5, 15)]
		[SerializeField]
		private string m_Text;

		#endregion


	}

}