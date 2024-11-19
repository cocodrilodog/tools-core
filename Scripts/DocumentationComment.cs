namespace CocodriloDog.Core {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// An object used to add documentation comments on any object.
	/// </summary>
	[Serializable]
	public class DocumentationComment {


		#region Public Properties

#if UNITY_EDITOR
		/// <summary>
		/// In the inspector, shows the <see cref="m_DocumentationComment"/> property field when is 
		/// <c>true</c>.
		/// </summary>
		public bool EditDocumentationComment {
			get => m_EditDocumentationComment;
			set => m_EditDocumentationComment = value;
		}
#endif

		#endregion


		#region Private Fields - Serialized

#if UNITY_EDITOR
		[SerializeField]
		private string m_Comment;
#endif

		#endregion


		#region Private Fields - Non Serialized

#if UNITY_EDITOR
		[NonSerialized]
		private bool m_EditDocumentationComment;
#endif

		#endregion


	}

}