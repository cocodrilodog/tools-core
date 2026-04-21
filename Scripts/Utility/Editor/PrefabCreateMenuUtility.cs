namespace CocodriloDog.App {

	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Utility class to create prefabs in the project panel.
	/// </summary>
	public class PrefabCreateMenuUtility {


		#region Public Static Methods
		
		public static void CreatePrefab(string prefabPath) {

			// Get the selected folder path, or default to the root Assets folder
			string folderPath = GetSelectedFolderPath() ?? "Assets";

			GameObject prefab = LoadPrefab(prefabPath);
			if (prefab == null) return;

			GameObject instance = GameObject.Instantiate(prefab);
			instance.name = prefab.name;

			string newPrefabPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{instance.name}.prefab");
			PrefabUtility.SaveAsPrefabAsset(instance, newPrefabPath);
			Object.DestroyImmediate(instance);

		}

		private static GameObject LoadPrefab(string prefabPath) {
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
			if (prefab == null) {
				Debug.LogError($"Prefab not found at path: {prefabPath}");
			}
			return prefab;
		}

		private static string GetSelectedFolderPath() {
			foreach (Object obj in Selection.GetFiltered(typeof(DefaultAsset), SelectionMode.Assets)) {
				string path = AssetDatabase.GetAssetPath(obj);
				if (AssetDatabase.IsValidFolder(path)) {
					return path;
				}
			}
			return null;
		}

		#endregion


	}

}