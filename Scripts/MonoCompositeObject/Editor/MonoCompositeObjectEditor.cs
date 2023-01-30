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

			// Gather the parents until the root
			var parent = (target as MonoCompositeObject).Parent;
			var parents = new List<MonoBehaviour>();
			while (parent != null) {
				parents.Add(parent);
				parent = parent is MonoCompositeObject ? (parent as MonoCompositeObject).Parent : null;
			}
			var rootParent = parents[parents.Count - 1] as MonoCompositeRoot;

			// Draw the buttons starting with the last one (the parent)
			for (int i = parents.Count - 1; i >= 0; i--) {
				if(parents[i] is MonoCompositeObject) {
					var nonRootParent = (MonoCompositeObject)parents[i];
					if (GUILayout.Button($"◂ {nonRootParent.ObjectName}", GUILayout.ExpandWidth(false))) {
						rootParent.SelectedMonoCompositeObject = nonRootParent;
					}
				} else if(parents[i] is MonoCompositeRoot) {
					if (GUILayout.Button($"◂ {parents[i].GetType().Name}", GUILayout.ExpandWidth(false))) {
						rootParent.SelectedMonoCompositeObject = null;
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