namespace CocodriloDog.Core {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

	/// <summary>
	/// Base class for editors of concrete subclasses of <see cref="MonoScriptableObject"/>.
	/// </summary>
	/// 
	/// <remarks>
	/// It allows the user to go back and inspect the chain of owner objects, as a usability feature. 
	/// It also disables the owner field so that it is not changed unintentionally.
	/// </remarks>
    public abstract class MonoScriptableObjectEditor : Editor {


		#region Unity Methods

		protected virtual void OnEnable() {
			if (target != null) { // <- This handles a very strange bug that happens when compiling any MonoScriptableObject
				ScriptProperty = serializedObject.FindProperty("m_Script");
				ObjectNameProperty = serializedObject.FindProperty("m_ObjectName");
			}
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			DisabledField(ScriptProperty);
			DrawBreadcrumbs();
			EditorGUILayout.PropertyField(ObjectNameProperty);
			serializedObject.ApplyModifiedProperties();
		}

		#endregion


		#region Private Properties

		private SerializedProperty ScriptProperty { get; set; }

		private SerializedProperty ObjectNameProperty { get; set; }

		#endregion


		#region Private Methods

		void DisabledField(SerializedProperty property, bool hasLabel = true) {
			EditorGUI.BeginDisabledGroup(true);
			if (hasLabel) {
				EditorGUILayout.PropertyField(property);
			} else {
				EditorGUILayout.PropertyField(property, GUIContent.none);
			}
			EditorGUI.EndDisabledGroup();
		}

		private void DrawBreadcrumbs() {
			EditorGUILayout.BeginHorizontal();
			var owner = (target as MonoScriptableObject).Owner;
			var owners = new List<Object>();
			while (owner != null) {
				owners.Add(owner);
				owner = owner is MonoScriptableObject ? (owner as MonoScriptableObject).Owner : null;
			}
			for (int i = owners.Count - 1; i >= 0; i--) {
				if(owners[i] is MonoScriptableObject) {
					var ownerMSO = (MonoScriptableObject)owners[i];
					if (GUILayout.Button($"◂ {ownerMSO.ObjectName}", GUILayout.ExpandWidth(false))) {
						(target as MonoScriptableObject).GetComponent<MonoScriptableRoot>().SelectedMonoScriptableObject = ownerMSO;
					}
				} else if(owners[i] is MonoScriptableRoot) {
					if (GUILayout.Button($"◂ {owners[i].name}", GUILayout.ExpandWidth(false))) {
						(target as MonoScriptableObject).GetComponent<MonoScriptableRoot>().SelectedMonoScriptableObject = null;
					}
				}
			}
			EditorGUI.BeginDisabledGroup(true);
			GUILayout.Button($"• {(target as MonoScriptableObject).ObjectName}", GUILayout.ExpandWidth(false));
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}

		#endregion


	}

}