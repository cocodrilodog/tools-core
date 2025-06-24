namespace CocodriloDog.Core.Examples {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Serialization;

	[AddComponentMenu("")]
	public class CompositeObjectReference_Example : MonoCompositeRoot {


		#region Unity Methods

		private IEnumerator Start() {

			Debug.Log("----------------");
			Debug.Log(m_FileReference.Value.Name);
			foreach (var fileReference in m_FileReferences) {
				Debug.Log(fileReference.Value.Name);
			}

			yield return new WaitForSeconds(2);
			if (m_Copy) {
				Instantiate(m_CopyPrefab);
			}

		}

		private void OnDestroy() {
			m_MyDisk.Dispose();
			m_OtherFile.Dispose();
			m_OtherFileNull?.Dispose();
			m_OtherFilesCompositeList.ForEach(f => f.Dispose());
			m_OtherFilesCompositeListEmpty.ForEach(f => f.Dispose());
			m_OtherFilesList.ForEach(f => f.Dispose());
			m_OtherFilesArray.ForEach(f => f.Dispose());
		}

		#endregion


		#region Private Fields

		[SerializeField]
		private GameObject m_CopyPrefab;

		[SerializeField]
		private bool m_Copy;

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

		[SerializeField]
		private bool m_LogReferences = true;

		[Tooltip("Find a file.")]
		[SerializeField]
		private CompositeObjectReference<FileBase> m_FileReference;

		[Tooltip("Find many files.")]
		[SerializeField]
		private List<CompositeObjectReference<FileBase>> m_FileReferences;

		#endregion


	}

}