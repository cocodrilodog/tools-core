namespace CocodriloDog.Core.Examples {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Serialization;

	[AddComponentMenu("")]
	public class CompositeObjectReference_Example : MonoCompositeRoot {


		#region Unity Methods

		private void Awake() {
			m_MyDisk.RegisterAsReferenceable(this);
			m_OtherFile.RegisterAsReferenceable(this);
			m_OtherFileNull?.RegisterAsReferenceable(this);
			m_OtherFilesCompositeList.ForEach(f => f.RegisterAsReferenceable(this));
			m_OtherFilesCompositeListEmpty.ForEach(f => f.RegisterAsReferenceable(this));
			m_OtherFilesList.ForEach(f => f.RegisterAsReferenceable(this));
			m_OtherFilesArray.ForEach(f => f.RegisterAsReferenceable(this));
		}

		private IEnumerator Start() {

			Debug.Log("----------------");
			Debug.Log(m_FileReference.Value.Name);
			foreach (var fileReference in m_FileReferences) {
				Debug.Log(fileReference.Value.Name);
			}

			Debug.Log(m_FileFromScriptableObject.Value.Name);

			// It is null on the prefab instance.
			if (m_FileFromOtherGO.Value != null) {
				Debug.Log(m_FileFromOtherGO.Value.Name);
			}
			// It is null on the prefab instance.
			if (m_FileFromOtherGO2.Value != null) {
				Debug.Log(m_FileFromOtherGO2.Value.Name);
			}

			yield return new WaitForSeconds(2);
			if (m_Copy) {
				Instantiate(m_CopyPrefab);
			}

		}

		private void OnDestroy() {
			m_MyDisk.UnregisterReferenceable(this);
			m_OtherFile.UnregisterReferenceable(this);
			m_OtherFileNull?.UnregisterReferenceable(this);
			m_OtherFilesCompositeList.ForEach(f => f.UnregisterReferenceable(this));
			m_OtherFilesCompositeListEmpty.ForEach(f => f.UnregisterReferenceable(this));
			m_OtherFilesList.ForEach(f => f.UnregisterReferenceable(this));
			m_OtherFilesArray.ForEach(f => f.UnregisterReferenceable(this));
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

		[Tooltip("Find a file in a ScriptableObject.")]
		[SerializeField]
		private CompositeObjectReference<FileBase> m_FileFromScriptableObject;

		[Tooltip("Find a file in another game object.")]
		[SerializeField]
		private CompositeObjectReference<FileBase> m_FileFromOtherGO;

		[Tooltip("Find a file in another game object.")]
		[SerializeField]
		private CompositeObjectReference<FileBase> m_FileFromOtherGO2;

		#endregion


	}

}