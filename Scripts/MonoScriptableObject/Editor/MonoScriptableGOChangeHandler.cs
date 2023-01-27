namespace CocodriloDog.Core {

	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// This script handles the case when properties are changed in <see cref="IMonoScriptableOwner"/>
	/// It will look for repeated references that point to the same <see cref="MonoScriptableObject"/>
	/// and recreate them so that they are unique. Additionally, it will remove <see cref="MonoScriptableObject"/>
	/// components that are no longer referenced by any <see cref="IMonoScriptableOwner"/>.
	/// </summary>
	/// 
	/// <remarks>
	/// The specific cases that are handled by this script are the following:
	/// 
	/// <list type="bullet">
	/// <item>
	///		<term>Adding List Or Array Item</term>
	///		<description>
	///			When an item is added, it will point to the same <see cref="MonoScriptableObject"/> as the 
	///			previous item. This script recreates it and now the new item points to a new and unique 
	///			<see cref="MonoScriptableObject"/>.
	///		</description>
	/// </item>
	/// <item>
	///		<term>Removing List Or Array Item</term>
	///		<description>
	///			When an item is removed, the <see cref="MonoScriptableObject"/> component will remain on the 
	///			<c>GameObject</c>. This script removes it.
	///		</description>
	/// </item>
	/// <item>
	///		<term>Pasting a <see cref="MonoScriptableObject"/></term>
	///		<description>
	///			When a <see cref="MonoScriptableObject"/> is pasted in a field, it is the same reference as the 
	///			original one. This script recreates it and makes it unique. If the field had a previous value, its
	///			corresponding <see cref="MonoScriptableObject"/> will remain on the <c>GameObject</c> and this script
	///			removes it.
	///		</description>
	/// </item>
	/// </list>
	/// </remarks>
	[InitializeOnLoad]
	public static class MonoScriptableGOChangeHandler {


		#region Public Static Methods

		/// <summary>
		/// Recreates repeated references of <see cref="MonoScriptableObject"/> and removed unowned
		/// <see cref="MonoScriptableObject"/> components from the <c>GameObject</c>.
		/// </summary>
		/// <param name="gameObject"></param>
		public static void UpdateMonoScriptableObjects(GameObject gameObject) {
			RecreateRepeatedReferences(gameObject);
			RemoveUnowned(gameObject);
		}

		#endregion


		#region Static Constructors

		static MonoScriptableGOChangeHandler() {
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
					// This will work when a IMonoScriptableOwner component changes any of its properties
					case ObjectChangeKind.ChangeGameObjectOrComponentProperties:

						stream.GetChangeGameObjectOrComponentPropertiesEvent(i, out var changeGameObjectOrComponent);
						var goOrComponent = EditorUtility.InstanceIDToObject(changeGameObjectOrComponent.instanceId);

						if (goOrComponent is IMonoScriptableOwner) {
							Debug.Log("CHANGED PROPERTIES");
							// Send the game object in which the change was originated
							UpdateMonoScriptableObjects((goOrComponent as Component).gameObject);
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
			var gameObjects = new List<GameObject>(GameObject.FindObjectsOfType<GameObject>());
			gameObjects.Remove(gameObject);
			gameObjects.Add(gameObject); // <- make it the last one so that the repetitions
										 // are more likely to be found on these one

			// Collect all owners because one MSO may have been copied from other game object
			var allOwners = new List<IMonoScriptableOwner>();

			// Isolate the owners that correspond to the gameObject for later use
			IMonoScriptableOwner[] gameObjectOwners = null;

			foreach (var go in gameObjects) {
				var owners = go.GetComponentsInChildren<IMonoScriptableOwner>();
				allOwners.AddRange(owners);
				if(go == gameObject) {
					gameObjectOwners = owners;
				}
			}

			// Collect all fields
			var fields = new List<MonoScriptableFieldBase>();
			foreach (var owner in allOwners) {
				fields.AddRange(owner.GetMonoScriptableFields());
			}

			// Check if fields have repeated references
			var ids = new List<int>();
			foreach (var field in fields) {
				if (field.ObjectBase != null) {
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

			// Confirm ownership only on the owners that are on the gameObject which is the one
			// that changed
			foreach(var owner in gameObjectOwners) {
				owner.ConfirmOwnership();
			}

		}

		private static  void RemoveUnowned(GameObject gameObject) {

			// Look for the owned objects
			var ownedObjects = new List<MonoScriptableObject>();
			var root = gameObject.GetComponent<MonoScriptableRoot>() as IMonoScriptableOwner;

			// Start on the root
			if (root != null) {

				// Look from root to leaves. Everyone else (that is not owned) must be destroyed.
				FindOwnedObjects(root.GetMonoScriptableFields());
				void FindOwnedObjects(MonoScriptableFieldBase[] fields) {
					foreach (var field in fields) {
						if (field.ObjectBase != null) {
							var ownedObject = field.ObjectBase;
							Debug.Log($"Owned object: {ownedObject.ObjectName}");
							ownedObjects.Add(ownedObject);
							// Recursion
							if (ownedObject is IMonoScriptableOwner) {
								FindOwnedObjects((ownedObject as IMonoScriptableOwner).GetMonoScriptableFields());
							}
						}
					}
				}

				// Get the currently attached objects
				var attachedObjects = gameObject.GetComponentsInChildren<MonoScriptableObject>();
				foreach (var attachedObject in attachedObjects) {
					Debug.Log($"Attached object: {attachedObject.ObjectName}");
					if (!ownedObjects.Contains(attachedObject)) {
						// Destroy the ones that are not owned
						Debug.Log($"Destroying: {attachedObject.ObjectName}");
						Undo.DestroyObjectImmediate(attachedObject);
					}
				}

			}

		}

		#endregion


	}
}

