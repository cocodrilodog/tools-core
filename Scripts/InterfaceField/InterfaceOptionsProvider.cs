namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using UnityEditor;
	using UnityEditor.Experimental.GraphView;
	using UnityEngine;

	/// <summary>
	/// Used by the <c>InterfaceFieldPropertyDrawer</c> to display the objects from the scene and the assets
	/// folder that implements the specified <see cref="m_InterfaceType"/>.
	/// </summary>
	public class InterfaceOptionsProvider : ScriptableObject, ISearchWindowProvider {


		#region Public Constructor

		public InterfaceOptionsProvider(Type interfaceType, Action<UnityEngine.Object> onSelectEntry) {
			m_InterfaceType = interfaceType;
			m_OnSelectEntry = onSelectEntry;
		}

		#endregion


		#region Public Methods

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) {

			// Display progress bar, in case it is a long wait
			const string progressTitle = "Building Search Tree";
			EditorUtility.DisplayProgressBar(progressTitle, "Please wait...", 0);

			// Create the entries list
			var entries = new List<SearchTreeEntry> {
				new SearchTreeGroupEntry(new GUIContent("Search Results"), 0)
			};

			// Search in scene
			var sceneObjects = new List<MonoBehaviour>(FindObjectsOfType<MonoBehaviour>())
				.Where(c => m_InterfaceType.IsAssignableFrom(c.GetType())).ToList();

			if (sceneObjects.Count > 0) {
				entries.Add(new SearchTreeGroupEntry(new GUIContent("Scene"), 1));
				for (int i = 0; i < sceneObjects.Count; i++) {
					var icon = AssetPreview.GetMiniThumbnail(sceneObjects[i]);
					entries.Add(new SearchTreeEntry(new GUIContent(sceneObjects[i].name, icon)) {
						level = 2,
						userData = sceneObjects[i]
					});
					EditorUtility.DisplayProgressBar(progressTitle, "Searching in the scene...", 0.33f * i / sceneObjects.Count);
				}
			}

			// Search for prefabs in the assets folder
			var prefabGUIDs = AssetDatabase.FindAssets("t:" + typeof(GameObject).Name);
			if (prefabGUIDs.Length > 0) {
				entries.Add(new SearchTreeGroupEntry(new GUIContent("Assets"), 1));
				for (int i = 0; i < prefabGUIDs.Length; i++) {
					var path = AssetDatabase.GUIDToAssetPath(prefabGUIDs[i]);
					var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
					if(prefab.TryGetComponent(m_InterfaceType, out var component)) {
						var icon = AssetPreview.GetMiniThumbnail(component);
						entries.Add(new SearchTreeEntry(new GUIContent(component.name, icon)) {
							level = 2,
							userData = component
						});
					}
					EditorUtility.DisplayProgressBar(progressTitle, "Searching for prefab assets...", 0.33f * i / prefabGUIDs.Length + 0.33f);
				}
			}

			// Search for scriptable objects in the assets folder
			var scriptableObjectGUIDs = AssetDatabase.FindAssets("t:" + typeof(ScriptableObject).Name);
			if (scriptableObjectGUIDs.Length > 0) {
				for (int i = 0; i < scriptableObjectGUIDs.Length; i++) {
					var path = AssetDatabase.GUIDToAssetPath(scriptableObjectGUIDs[i]);
					var scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
					if (m_InterfaceType.IsAssignableFrom(scriptableObject.GetType())) {
						var icon = AssetPreview.GetMiniThumbnail(scriptableObject);
						entries.Add(new SearchTreeEntry(new GUIContent(scriptableObject.name, icon)) {
							level = 2,
							userData = scriptableObject
						});
					}
					EditorUtility.DisplayProgressBar(progressTitle, "Searching for ScriptableObject assets...", 0.33f * i / scriptableObjectGUIDs.Length + 0.66f);
				}
			}

			// Add a "None" option
			Texture2D deleteIcon = EditorGUIUtility.IconContent("d_P4_DeletedLocal").image as Texture2D;
			entries.Add(new SearchTreeEntry(new GUIContent("None", deleteIcon)) {
				level = 1,
				userData = null
			});

			// Clear progress bar
			EditorUtility.DisplayProgressBar(progressTitle, "Done!", 1);
			EditorUtility.ClearProgressBar();

			return entries;

		}

		public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context) {
			var selectedObject = SearchTreeEntry.userData as UnityEngine.Object;
			m_OnSelectEntry?.Invoke(selectedObject);
			return true;
		}

		#endregion


		#region Private Fields

		private Type m_InterfaceType;

		private Action<UnityEngine.Object> m_OnSelectEntry;

		#endregion


	}

}