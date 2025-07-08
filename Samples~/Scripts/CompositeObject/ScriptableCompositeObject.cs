namespace CocodriloDog.Core.Examples {

	using UnityEngine;

	[CreateAssetMenu(fileName = "CompositeObjectReference_ScriptableObject", menuName = "Cocodrilo Dog/Core/Examples/ScriptableCompositeObject")]
	public class ScriptableCompositeObject : ScriptableCompositeRoot {


		#region Unity Methods

		private void OnEnable() {
			m_MyDisk.RegisterAsReferenceable(this);
			MonoUpdater.OnDestroyEv -= MonoUpdater_OnDestroyEv;
			MonoUpdater.OnDestroyEv += MonoUpdater_OnDestroyEv;
		}

		private void MonoUpdater_OnDestroyEv() {
			m_MyDisk.UnregisterReferenceable(this);
		}

		#endregion


		#region Private Fields

		[SerializeReference]
		private Folder m_MyDisk;

		#endregion


	}

}