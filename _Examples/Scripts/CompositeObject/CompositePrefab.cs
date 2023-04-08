using UnityEditor;
using UnityEngine;

public class MyPrefabEditor {
	[MenuItem("GameObject/Apply Changes to Prefab", true)]
	static bool ValidateApplyChangesToPrefab() {
		return Selection.activeGameObject != null && PrefabUtility.IsPartOfPrefabInstance(Selection.activeGameObject);
	}

	[MenuItem("GameObject/Apply Changes to Prefab", false, 0)]
	static void ApplyChangesToPrefab() {
		// Custom implementation of apply changes to prefab
		Debug.Log("Applying changes to prefab");
	}
}
