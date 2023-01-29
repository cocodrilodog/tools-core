namespace CocodriloDog.Core {

	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// This script handles the case when properties are changed in <see cref="IMonoCompositeParent"/>
	/// It will look for repeated references that point to the same <see cref="MonoCompositeObject"/>
	/// and recreate them so that they are unique. Additionally, it will remove <see cref="MonoCompositeObject"/>
	/// components that are no longer referenced by any <see cref="IMonoCompositeParent"/>.
	/// </summary>
	/// 
	/// <remarks>
	/// The specific cases that are handled by this script are the following:
	/// 
	/// <list type="bullet">
	/// <item>
	///		<term>Adding List Or Array Item</term>
	///		<description>
	///			When an item is added, it will point to the same <see cref="MonoCompositeObject"/> as the 
	///			previous item. This script recreates it and now the new item points to a new and unique 
	///			<see cref="MonoCompositeObject"/>.
	///		</description>
	/// </item>
	/// <item>
	///		<term>Removing List Or Array Item</term>
	///		<description>
	///			When an item is removed, the <see cref="MonoCompositeObject"/> component will remain on the 
	///			<c>GameObject</c>. This script removes it.
	///		</description>
	/// </item>
	/// <item>
	///		<term>Pasting a <see cref="MonoCompositeObject"/></term>
	///		<description>
	///			When a <see cref="MonoCompositeObject"/> is pasted in a field, it is the same reference as the 
	///			original one. This script recreates it and makes it unique. If the field had a previous value, its
	///			corresponding <see cref="MonoCompositeObject"/> will remain on the <c>GameObject</c> and this script
	///			removes it.
	///		</description>
	/// </item>
	/// </list>
	/// </remarks>
	[InitializeOnLoad]
	public static class MonoCompositeGOChangeHandler {


		#region Public Static Methods

		/// <summary>
		/// Recreates repeated references of <see cref="MonoCompositeObject"/> and removes orphan
		/// <see cref="MonoCompositeObject"/> components from the <c>GameObject</c>.
		/// </summary>
		/// <param name="gameObject"></param>
		public static void UpdateMonoCompositeObjects(GameObject gameObject) {
			RecreateRepeatedReferences(gameObject);
			RemoveOrphans(gameObject);
		}

		#endregion


		#region Static Constructors

		static MonoCompositeGOChangeHandler() {
			Undo.undoRedoPerformed += OnUndo;
			ObjectChangeEvents.changesPublished += OnChangesPublished;
		}

		#endregion


		#region Event Handlers

		/// <summary>
		/// Undo is fired first so I use this flag for no change handling to occur during an undo operation,
		/// otherwise strange things will happen.
		/// </summary>
		private static bool m_IgnoreChanges;

		private static void OnUndo() {
			IgnoreChanges();
		}

		private static void OnChangesPublished(ref ObjectChangeEventStream stream) {

			if (m_IgnoreChanges) {
				m_IgnoreChanges = false;
				return;
			}

			for (int i = 0; i < stream.length; ++i) {
				switch (stream.GetEventType(i)) {

					// This will work when a IMonoCompositeParent component changes any of its properties
					case ObjectChangeKind.ChangeGameObjectStructure:
						// TODO: Remove composite children when the root is removed
						stream.GetChangeGameObjectStructureEvent(i, out var changeGameObjectStructure);
						var go = EditorUtility.InstanceIDToObject(changeGameObjectStructure.instanceId) as GameObject;
						// It is a game object and it has MonoCompositeObjects. This case will check if the root was removed
						// ad if so, remove all MonoCompositeObjects
						if (go != null && go.GetComponentsInChildren<MonoCompositeObject>().Length > 0) {
							Debug.Log("GAME OBJECT STRUCTURE CHANGED");
							RemoveOrphans(go);
						}
						break;

					// This will work when a IMonoCompositeParent component changes any of its properties
					case ObjectChangeKind.ChangeGameObjectOrComponentProperties:

						stream.GetChangeGameObjectOrComponentPropertiesEvent(i, out var changeGameObjectOrComponent);
						var goOrComponent = EditorUtility.InstanceIDToObject(changeGameObjectOrComponent.instanceId);

						if (goOrComponent is IMonoCompositeParent) {
							Debug.Log("CHANGED PROPERTIES");
							// Send the game object in which the change was originated
							UpdateMonoCompositeObjects((goOrComponent as Component).gameObject);
						}

						break;
				}
			}

		}

		#endregion


		#region Private Static Methods

		private static void IgnoreChanges() => m_IgnoreChanges = true;

		private static void RecreateRepeatedReferences(GameObject gameObject) {

			// Collect all game objects in the scene
			//Debug.Log("COLLECT GOs");
			var gameObjects = new List<GameObject>(GameObject.FindObjectsOfType<GameObject>());
			gameObjects.Remove(gameObject);
			gameObjects.Add(gameObject); // <- make it the last one so that the repetitions
										 // are more likely to be found on these one

			// Collect all parents because one MSO may have been copied from other game object
			var allParents = new List<IMonoCompositeParent>();

			// Isolate the parents that correspond to the gameObject for later use
			IMonoCompositeParent[] gameObjectParents = null;

			foreach (var go in gameObjects) {
				//Debug.Log($"go: {go}");
				var parents = go.GetComponentsInChildren<IMonoCompositeParent>();
				allParents.AddRange(parents);
				if(go == gameObject) {
					gameObjectParents = parents;
				}
			}

			// Collect all fields
			//Debug.Log("COLLECT FIELDS");
			var fields = new List<MonoCompositeFieldBase>();
			foreach (var parent in allParents) {
				fields.AddRange(parent.GetChildren());
			}

			// Check if fields have repeated references
			var ids = new List<int>();
			foreach (var field in fields) {
				if (field.ObjectBase != null) {
					//Debug.Log($"*** field: {field.ObjectBase.ObjectName}");
					if (!ids.Contains(field.ObjectBase.GetInstanceID())) {
						ids.Add(field.ObjectBase.GetInstanceID());
					} else {
						Debug.Log($"Repeated: {field.ObjectBase.ObjectName}");
						// When recreating a field, ChangeGameObjectOrComponentProperties will be invoked again,
						// causing any new repetition to be handled immediatly. A field containing a child field
						// with a child field and so on... may cause several sequenced calls.
						field.Recreate(gameObject);
					}
				}
			}

			// Confirm children only on the parents that are on the gameObject which is the one
			// that changed
			foreach(var parent in gameObjectParents) {
				parent.ConfirmChildren();
			}

		}

		private static  void RemoveOrphans(GameObject gameObject) {

			// Look for the owned objects
			var ownedObjects = new List<MonoCompositeObject>();
			var root = gameObject.GetComponent<MonoCompositeRoot>() as IMonoCompositeParent;

			// Start on the root
			if (root != null) {

				// Look from root to leaves. Everyone else (that is not owned) must be destroyed.
				FindOwnedObjects(root.GetChildren());
				void FindOwnedObjects(MonoCompositeFieldBase[] fields) {
					foreach (var field in fields) {
						if (field.ObjectBase != null) {
							var ownedObject = field.ObjectBase;
							Debug.Log($"Owned object: {ownedObject.ObjectName}");
							ownedObjects.Add(ownedObject);
							// Recursion
							if (ownedObject is IMonoCompositeParent) {
								FindOwnedObjects((ownedObject as IMonoCompositeParent).GetChildren());
							}
						}
					}
				}

				// Get the currently attached objects
				var attachedObjects = gameObject.GetComponentsInChildren<MonoCompositeObject>();
				foreach (var attachedObject in attachedObjects) {
					Debug.Log($"Attached object: {attachedObject.ObjectName}");
					if (!ownedObjects.Contains(attachedObject)) {
						// Destroy the ones that are not owned
						Debug.Log($"Destroying: {attachedObject.ObjectName}");
						Undo.DestroyObjectImmediate(attachedObject);
					}
				}

			} else {
				// The root was removed, so let us remove all!
				Debug.Log($"Destroying all MonoComposite objects from {gameObject}");
				var attachedObjects = gameObject.GetComponentsInChildren<MonoCompositeObject>();
				foreach (var attachedObject in attachedObjects) {
					Debug.Log($"Destroying: {attachedObject.ObjectName}");
					Undo.DestroyObjectImmediate(attachedObject);
				}
			}

		}

		#endregion


	}
}

