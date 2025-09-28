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

			Debug.Log(m_FileFromCurrentComponent.Value.Name);

			foreach (var fileReference in m_FilesFromCurrentComponent) {
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

			Debug.Log(m_FileFromFixedSource.Value.Name);

			Debug.Log(m_FileFromFixedHiddenSource.Value.Name);

			foreach (var fileReference in m_FilesFromFixedSource) {
				Debug.Log(fileReference.Value.Name);
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

		private void OnValidate() {
			
			m_FileFromFixedSource.Source = this;
			m_FileFromFixedSource.SetMode(CompositeObjectReferenceMode.CannotChooseSource_ShowSource);
			
			m_FileFromFixedHiddenSource.Source = this;
			m_FileFromFixedHiddenSource.SetMode(CompositeObjectReferenceMode.CannotChooseSource_HideSource);
			
			m_FilesFromFixedSource.ForEach(fr => fr.Source = this);
			m_FilesFromFixedSource.ForEach(fr => fr.SetMode(CompositeObjectReferenceMode.CannotChooseSource_ShowSource));

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

		[Tooltip("File reference field with default settings.")]
		[SerializeField]
		private CompositeObjectReference<FileBase> m_FileFieldWithDefaultSettings;

		[Tooltip("Find a file in the current component.")]
		[SerializeField]
		private CompositeObjectReference<FileBase> m_FileFromCurrentComponent;

		[Tooltip("Find many files in the current component.")]
		[SerializeField]
		private List<CompositeObjectReference<FileBase>> m_FilesFromCurrentComponent;

		[Tooltip("Find a file in a ScriptableObject.")]
		[SerializeField]
		private CompositeObjectReference<FileBase> m_FileFromScriptableObject;

		[Tooltip("Find a file in another game object.")]
		[SerializeField]
		private CompositeObjectReference<FileBase> m_FileFromOtherGO;

		[Tooltip("Find a file in another game object.")]
		[SerializeField]
		private CompositeObjectReference<FileBase> m_FileFromOtherGO2;

		[Tooltip("Find a file in a fixed source.")]
		[SerializeField]
		private CompositeObjectReference<FileBase> m_FileFromFixedSource;

		[Tooltip("Find a file in a fixed and hidden source.")]
		[SerializeField]
		private CompositeObjectReference<FileBase> m_FileFromFixedHiddenSource;

		[Tooltip("Find many files in a fixed source.")]
		[SerializeField]
		private List<CompositeObjectReference<FileBase>> m_FilesFromFixedSource;

		#endregion


	}

}