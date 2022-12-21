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
							Debug.Log($"Create GameObject: {gameObject} in scene {createGameObjectHierarchyEvent.scene}.");
							monoScriptableOwner.UpdateMonoScriptableObjects();
						}

						break;

					case ObjectChangeKind.ChangeGameObjectStructureHierarchy:

						stream.GetChangeGameObjectStructureHierarchyEvent(i, out var changeGameObjectStructureHierarchy);
						gameObject = EditorUtility.InstanceIDToObject(changeGameObjectStructureHierarchy.instanceId) as GameObject;

						monoScriptableOwner = gameObject.GetComponent<IMonoScriptableOwner>();
						if (monoScriptableOwner != null) {
							Debug.Log($"Change GameObject hierarchy: {gameObject} in scene {changeGameObjectStructureHierarchy.scene}.");
							monoScriptableOwner.UpdateMonoScriptableObjects();
						}

						break;

					case ObjectChangeKind.ChangeGameObjectStructure:

						stream.GetChangeGameObjectStructureEvent(i, out var changeGameObjectStructure);
						gameObject = EditorUtility.InstanceIDToObject(changeGameObjectStructure.instanceId) as GameObject;

						monoScriptableOwner = gameObject.GetComponent<IMonoScriptableOwner>();
						if (monoScriptableOwner != null) {
							Debug.Log($"Change GameObject structure: {gameObject} in scene {changeGameObjectStructure.scene}.");
							monoScriptableOwner.UpdateMonoScriptableObjects();
						}

						break;
;
				}
			}

		}

		#endregion


	}
}

