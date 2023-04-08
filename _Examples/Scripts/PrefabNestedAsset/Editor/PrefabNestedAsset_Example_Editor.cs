namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(PrefabNestedAsset_Example))]
	public class PrefabNestedAsset_Example_Editor : Editor {


		#region Unity Methods

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			serializedObject.Update();

			if(GUILayout.Button("Add Nested Asset")) {

				var prefab = serializedObject.FindProperty("m_Prefab").objectReferenceValue as GameObject;
				var nestedAsset = CreateInstance<NestedAsset>();
				nestedAsset.name = "NestedAsset";

				AssetDatabase.AddObjectToAsset(nestedAsset, prefab);
				serializedObject.FindProperty("m_NestedAsset").objectReferenceValue = nestedAsset;

			}

			serializedObject.ApplyModifiedProperties();

		}

		#endregion


	}

}