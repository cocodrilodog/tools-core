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
	/// <item>
	///		<term>Removing a <see cref="MonoCompositeRoot"/></term>
	///		<description>
	///			When a <see cref="MonoCompositeRoot"/> is removed, this will check if there are any orphan 
	///			<see cref="MonoCompositeObject"/> left.
	///		</description>
	/// </item>
	/// </list>
	/// </remarks>
	[InitializeOnLoad]
	public static class MonoCompositeGOChangeHandler {


		#region Static Constructors

		static MonoCompositeGOChangeHandler() {
			Undo.undoRedoPerformed += OnUndo;
			ObjectChangeEvents.changesPublished += OnChangesPublished;
		}

		#endregion


		#region Event Handlers

		private static void OnUndo() {
			IgnoreAllChanges();
		}

		private static void OnChangesPublished(ref ObjectChangeEventStream stream) {

			if (m_IgnoreAllChanges) {
				m_IgnoreAllChanges = false;
				return;
			}

			for (int i = 0; i < stream.length; ++i) {
				switch (stream.GetEventType(i)) {

					// This will work when a IMonoCompositeParent component changes any of its properties
					case ObjectChangeKind.ChangeGameObjectStructure: {

							stream.GetChangeGameObjectStructureEvent(i, out var changeGameObjectStructure);
							var go = EditorUtility.InstanceIDToObject(changeGameObjectStructure.instanceId) as GameObject;

							// It is a game object and it has MonoCompositeObjects. This will check if a root was removed
							// ad if so, remove all of its former MonoCompositeObjects children
							if (go != null && go.GetComponentsInChildren<MonoCompositeObject>().Length > 0) {
								Debug.Log("GAME OBJECT STRUCTURE CHANGED");
								RemoveOrphans(go);
							}

						}
						break;

					// This will work when a IMonoCompositeParent component changes any of its properties
					case ObjectChangeKind.ChangeGameObjectOrComponentProperties: {

							stream.GetChangeGameObjectOrComponentPropertiesEvent(i, out var changeGameObjectOrComponent);
							var goOrComponent = EditorUtility.InstanceIDToObject(changeGameObjectOrComponent.instanceId);

							if (goOrComponent is IMonoCompositeParent) {
								Debug.Log("CHANGED PROPERTIES");
								// Send the game object in which the change was originated
								var go = (goOrComponent as Component).gameObject;
								// If an array added an element or an element was pasted, we check for repetition
								RecreateRepeatedReferences(go);
								// If an array element was removed, we need to clean
								RemoveOrphans(go);
							}

						}
						break;
				}
			}

		}

		#endregion


		#region Private Fields

		/// <summary>
		/// Undo is fired first so I use this flag for no change handling to occur during an undo operation,
		/// otherwise strange things will happen.
		/// </summary>
		private static bool m_IgnoreAllChanges;

		#endregion


		#region Private Static Methods

		private static void IgnoreAllChanges() => m_IgnoreAllChanges = true;

		private static void RecreateRepeatedReferences(GameObject gameObject) {

			// Collect all game objects in the scene
			//Debug.Log("COLLECT GOs");
			var gameObjects = new List<GameObject>(GameObject.FindObjectsOfType<GameObject>());
			gameObjects.Remove(gameObject);
			gameObjects.Add(gameObject); // <- make it the last one so that the repetitions
										 // are more likely to be found on these one

			// Collect all parents because one MCO may have been copied from other game object
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

		/// <summary>
		/// Removes the <see cref="MonoCompositeObject"/> that have no parent from the <paramref name="gameObject"/>
		/// </summary>
		/// <param name="gameObject">The game object to check</param>
		private static void RemoveOrphans(GameObject gameObject) {

			// Look for all the owned children
			var children = new List<MonoCompositeObject>();
			var roots = gameObject.GetComponents<MonoCompositeRoot>();

			// Find all the children
			foreach (var root in roots) {
				// Start on the root
				if (root != null) {
					// Look from root to leaves. Everyone else (that is not owned) must be destroyed.
					FindChildren(root.GetChildren());
					void FindChildren(MonoCompositeFieldBase[] fields) {
						foreach (var field in fields) {
							if (field.ObjectBase != null) {
								var child = field.ObjectBase;
								//Debug.Log($"Child object: {child.ObjectName}");
								children.Add(child);
								// Recursion
								if (child is IMonoCompositeParent) {
									FindChildren((child as IMonoCompositeParent).GetChildren());
								}
							}
						}
					}
				}
			}

			// Get the currently attached objects
			var attachedObjects = gameObject.GetComponentsInChildren<MonoCompositeObject>();
			foreach (var attachedObject in attachedObjects) {
				//Debug.Log($"Attached object: {attachedObject.ObjectName}");
				if (!children.Contains(attachedObject)) {
					// Destroy the ones that are not children
					Debug.Log($"Destroying: {attachedObject.ObjectName}");
					Undo.DestroyObjectImmediate(attachedObject);
				}
			}

		}

		#endregion


	}
}

