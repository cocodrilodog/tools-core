namespace CocodriloDog.Core {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

	/// <summary>
	/// Base class for editors of concrete subclasses of <see cref="MonoCompositeObject"/>.
	/// </summary>
	/// 
	/// <remarks>
	/// It allows the user to go back and inspect the chain of owner objects, as a usability feature. 
	/// It also disables the owner field so that it is not changed unintentionally.
	/// </remarks>
    public abstract class MonoCompositeObjectEditor : Editor {


		#region Unity Methods

		protected virtual void OnEnable() {
			if (target != null) { // <- This handles a very strange bug that happens when compiling any MonoCompositeObject
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
			var owner = (target as MonoCompositeObject).Parent;
			var owners = new List<MonoBehaviour>();
			while (owner != null) {
				owners.Add(owner);
				owner = owner is MonoCompositeObject ? (owner as MonoCompositeObject).Parent : null;
			}
			for (int i = owners.Count - 1; i >= 0; i--) {
				if(owners[i] is MonoCompositeObject) {
					var ownerMSO = (MonoCompositeObject)owners[i];
					if (GUILayout.Button($"◂ {ownerMSO.ObjectName}", GUILayout.ExpandWidth(false))) {
						(target as MonoCompositeObject).GetComponent<MonoCompositeRoot>().SelectedMonoCompositeObject = ownerMSO;
					}
				} else if(owners[i] is MonoCompositeRoot) {
					if (GUILayout.Button($"◂ {owners[i].name}", GUILayout.ExpandWidth(false))) {
						(target as MonoCompositeObject).GetComponent<MonoCompositeRoot>().SelectedMonoCompositeObject = null;
					}
				}
			}
			EditorGUI.BeginDisabledGroup(true);
			GUILayout.Button($"• {(target as MonoCompositeObject).ObjectName}", GUILayout.ExpandWidth(false));
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}

		#endregion


	}

}