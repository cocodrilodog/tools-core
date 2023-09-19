namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(SerializeReference_Example))]
	public class SerializeReference_Example_Editor : Editor {

		private void OnEnable() {
			CatAProperty = serializedObject.FindProperty("CatA");
			CatBProperty = serializedObject.FindProperty("CatB");
		}

		public override void OnInspectorGUI() {
			//base.OnInspectorGUI();
			
			serializedObject.Update();

			EditorGUILayout.PropertyField(CatAProperty, true);

			if(GUILayout.Button("Cat A: Null")) {
				CatAProperty.managedReferenceValue = null;
				Debug.Log($"A:managedReferenceId:{CatAProperty.managedReferenceId}");
			}
			if (GUILayout.Button("Cat A: New Cat")) {
				CatAProperty.managedReferenceValue = new Cat("Audio Ninja");
				Debug.Log($"A:managedReferenceId:{CatAProperty.managedReferenceId}");
			}
			if (GUILayout.Button("Cat A: New MotherCat")) {
				CatAProperty.managedReferenceValue = new CatMother("Moana");
				Debug.Log($"A:managedReferenceId:{CatAProperty.managedReferenceId}");
			}

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(CatBProperty, true);

			if (GUILayout.Button("Cat B[0]: Null")) {
				CatBProperty.GetArrayElementAtIndex(0).managedReferenceValue = null;
			}
			if (GUILayout.Button("Cat B[0]: New Cat")) {
				CatBProperty.GetArrayElementAtIndex(0).managedReferenceValue = new Cat("Andino Kid");
			}
			if (GUILayout.Button("Cat B[0]: New MotherCat")) {
				CatBProperty.GetArrayElementAtIndex(0).managedReferenceValue = new CatMother("JoJo Siwa");
			}

			EditorGUILayout.Space();

			if (GUILayout.Button("Cat A and B[0]: New Cat")) {
				CatBProperty.GetArrayElementAtIndex(0).managedReferenceValue = 
					CatAProperty.managedReferenceValue = new Cat("Boom Fighters");
				Debug.Log($"A:managedReferenceId:{CatAProperty.managedReferenceId}");
				Debug.Log($"B:managedReferenceId:{CatBProperty.managedReferenceId}");
			}
			if (GUILayout.Button("Cat A and B[0]: New Mother Cat")) {
				CatBProperty.GetArrayElementAtIndex(0).managedReferenceValue =
					CatAProperty.managedReferenceValue = new CatMother("Beat Boy");
				Debug.Log($"A:managedReferenceId:{CatAProperty.managedReferenceId}");
				Debug.Log($"B:managedReferenceId:{CatBProperty.managedReferenceId}");
			}

			serializedObject.ApplyModifiedProperties();

		}

		private SerializedProperty CatAProperty { get; set; }

		private SerializedProperty CatBProperty { get; set; }

	}
}
