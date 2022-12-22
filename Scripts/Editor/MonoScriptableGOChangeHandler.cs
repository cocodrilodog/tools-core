namespace CocodriloDog.Core {

	using System.Text;
	using UnityEditor;
	using UnityEngine;

	[InitializeOnLoad]
	public static class MonoScriptableGOChangeHandler {


		#region Static Constructors

		static MonoScriptableGOChangeHandler() {
			ObjectChangeEvents.changesPublished += ChangesPublished;
		}

		#endregion


		#region Event Handlers

		private static void ChangesPublished(ref ObjectChangeEventStream stream) {

			GameObject gameObject;
			IMonoScriptableOwner monoScriptableOwner;

			for (int i = 0; i < stream.length; ++i) {
				switch (stream.GetEventType(i)) {

					case ObjectChangeKind.CreateGameObjectHierarchy:

						stream.GetCreateGameObjectHierarchyEvent(i, out var createGameObjectHierarchyEvent);
						gameObject = EditorUtility.InstanceIDToObject(createGameObjectHierarchyEvent.instanceId) as GameObject;

						monoScriptableOwner = gameObject.GetComponent<IMonoScriptableOwner>();
						if (monoScriptableOwner != null) {
							monoScriptableOwner.RecreateMonoScriptableObjects();
						}

						break;

					case ObjectChangeKind.ChangeGameObjectStructureHierarchy:

						stream.GetChangeGameObjectStructureHierarchyEvent(i, out var changeGameObjectStructureHierarchy);
						gameObject = EditorUtility.InstanceIDToObject(changeGameObjectStructureHierarchy.instanceId) as GameObject;

						monoScriptableOwner = gameObject.GetComponent<IMonoScriptableOwner>();
						if (monoScriptableOwner != null) {
							monoScriptableOwner.RecreateMonoScriptableObjects();
						}

						break;

					case ObjectChangeKind.ChangeGameObjectStructure:

						stream.GetChangeGameObjectStructureEvent(i, out var changeGameObjectStructure);
						gameObject = EditorUtility.InstanceIDToObject(changeGameObjectStructure.instanceId) as GameObject;

						monoScriptableOwner = gameObject.GetComponent<IMonoScriptableOwner>();
						if (monoScriptableOwner != null) {
							monoScriptableOwner.RecreateMonoScriptableObjects();
						}

						break;

					case ObjectChangeKind.ChangeGameObjectOrComponentProperties:

						stream.GetChangeGameObjectOrComponentPropertiesEvent(i, out var changeGameObjectOrComponent);
						var goOrComponent = EditorUtility.InstanceIDToObject(changeGameObjectOrComponent.instanceId);

						if (goOrComponent is IMonoScriptableOwner) {
							((IMonoScriptableOwner)goOrComponent).ValidateMonoScriptableArrayOrLists();
						}

						break;

					case ObjectChangeKind.ChangeAssetObjectProperties:

						stream.GetChangeAssetObjectPropertiesEvent(i, out var changeAssetObjectPropertiesEvent);
						var changeAsset = EditorUtility.InstanceIDToObject(changeAssetObjectPropertiesEvent.instanceId);

						if (changeAsset is IMonoScriptableOwner) {
							((IMonoScriptableOwner)changeAsset).ValidateMonoScriptableArrayOrLists();
						}

						break;

				}
			}

		}

		#endregion


	}
}

