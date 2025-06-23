namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Serialization;

	[AddComponentMenu("")]
	public class CompositeObjectReference_Example : MonoCompositeRoot {


		#region Unity Methods

		private void Start() {
			Debug.Log(m_FileReference.Value.Name);
			foreach(var fileReference in m_FileReferences) {
				Debug.Log(fileReference.Value.Name);
			}
		}

		#endregion


		#region Private Fields

		[Header("Files")]

		[SerializeReference]
		private Folder m_MyDisk;

		[SerializeReference]
		private FileBase m_OtherFile;

		[SerializeReference]
		private FileBase m_OtherFileNull;

		[FormerlySerializedAs("m_OtherFiles")]
		[SerializeField]
		private CompositeList<FileBase> m_OtherFilesCompositeList;

		[FormerlySerializedAs("m_OtherFilesEmpty")]
		[SerializeField]
		private CompositeList<FileBase> m_OtherFilesCompositeListEmpty;

		[SerializeReference]
		private List<FileBase> m_OtherFilesList;

		[SerializeReference]
		private List<FileBase> m_OtherFilesArray;

		[Header("References")]

		[Tooltip("Find a file.")]
		[SerializeField]
		private CompositeObjectReference<FileBase> m_FileReference;

		[Tooltip("Find many files.")]
		[SerializeField]
		private List<CompositeObjectReference<FileBase>> m_FileReferences;

		#endregion


	}

}