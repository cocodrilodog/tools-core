namespace CocodriloDog.Core {

	using System.Text;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// This script handles the different cases in which <see cref="MonoScriptableObject"/>s can have duplicated
	/// references. This is, when two or more fields are pointing to the same MonoScriptableObject instance.
	/// </summary>
	/// 
	/// <remarks>
	/// This happens when the owner game object is duplicated, when an owner component is copied/pasted and when
	/// a <see cref="MonoScriptableObject"/> array size is increased and the items are duplicated.
	/// 
	/// Components that own <see cref="MonoScriptableObject"/> properties should implement <see cref="IMonoScriptableOwner"/>
	/// to replace the duplicated references with new <see cref="MonoScriptableObject"/> instances.
	/// 
	/// This script invokes <see cref="IMonoScriptableOwner"/> methods in key moments in which the Concrete 
	/// <see cref="IMonoScriptableOwner"/> implementations apply their own logic to replace the objects.
	/// </remarks>

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

					// This will work when a game object that has a IMonoScriptableOwner component is duplicated
					case ObjectChangeKind.CreateGameObjectHierarchy:

						stream.GetCreateGameObjectHierarchyEvent(i, out var createGameObjectHierarchyEvent);
						gameObject = EditorUtility.InstanceIDToObject(createGameObjectHierarchyEvent.instanceId) as GameObject;

						monoScriptableOwner = gameObject.GetComponent<IMonoScriptableOwner>();
						if (monoScriptableOwner != null) {
							monoScriptableOwner.RecreateMonoScriptableObjects();
						}

						break;

					// This will work when a IMonoScriptableOwner component is pasted on the game object, while existing in the scene
					case ObjectChangeKind.ChangeGameObjectStructureHierarchy:

						stream.GetChangeGameObjectStructureHierarchyEvent(i, out var changeGameObjectStructureHierarchy);
						gameObject = EditorUtility.InstanceIDToObject(changeGameObjectStructureHierarchy.instanceId) as GameObject;

						monoScriptableOwner = gameObject.GetComponent<IMonoScriptableOwner>();
						if (monoScriptableOwner != null) {
							monoScriptableOwner.RecreateMonoScriptableObjects();
						}

						break;

					// This will work when a IMonoScriptableOwner component is pasted on the game object (this hasn't been tested, but
					// it is likely to happen in a prefab)
					case ObjectChangeKind.ChangeGameObjectStructure:

						stream.GetChangeGameObjectStructureEvent(i, out var changeGameObjectStructure);
						gameObject = EditorUtility.InstanceIDToObject(changeGameObjectStructure.instanceId) as GameObject;

						monoScriptableOwner = gameObject.GetComponent<IMonoScriptableOwner>();
						if (monoScriptableOwner != null) {
							monoScriptableOwner.RecreateMonoScriptableObjects();
						}

						break;

					// This will work when a IMonoScriptableOwner component changes any of its properties
					case ObjectChangeKind.ChangeGameObjectOrComponentProperties:

						stream.GetChangeGameObjectOrComponentPropertiesEvent(i, out var changeGameObjectOrComponent);
						var goOrComponent = EditorUtility.InstanceIDToObject(changeGameObjectOrComponent.instanceId);

						if (goOrComponent is IMonoScriptableOwner) {
							((IMonoScriptableOwner)goOrComponent).RecreateRepeatedMonoScriptableArrayOrListItems();
						}

						break;

					// This will work when a IMonoScriptableOwner asset changes any of its properties
					case ObjectChangeKind.ChangeAssetObjectProperties:

						stream.GetChangeAssetObjectPropertiesEvent(i, out var changeAssetObjectPropertiesEvent);
						var changeAsset = EditorUtility.InstanceIDToObject(changeAssetObjectPropertiesEvent.instanceId);

						if (changeAsset is IMonoScriptableOwner) {
							((IMonoScriptableOwner)changeAsset).RecreateRepeatedMonoScriptableArrayOrListItems();
						}

						break;

				}
			}

		}

		#endregion


	}
}

