namespace CocodriloDog.Core.Examples {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[Serializable]
	public class Folder : FileBase {


		#region Private Fields

		[SerializeReference]
		public List<FileBase> m_Files;

		#endregion


	}

}