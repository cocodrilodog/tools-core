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
	/// It allows the user to go back and inspect to the owner object, as a  usability feature. 
	/// It also disables the owner field so that it is not changed unintentionally.
	/// </remarks>
    public abstract class MonoScriptableObjectEditor : Editor {


		#region Unity Methods

		protected virtual void OnEnable() {
			ScriptProperty = serializedObject.FindProperty("m_Script");
			NameProperty = serializedObject.FindProperty("m_Name");
			OwnerProperty = serializedObject.FindProperty("m_Owner");
		}

		public override void OnInspectorGUI() {

			serializedObject.Update();

			DisabledField(ScriptProperty);

			EditorGUILayout.BeginHorizontal();
			var owner = (target as MonoScriptableObject).Owner;
			var owners = new List<Object>();
			while(owner != null) {
				owners.Add(owner);
				owner = owner is MonoScriptableObject ? (owner as MonoScriptableObject).Owner : null;
			}
			for(int i = owners.Count - 1; i >= 0; i--) {
				if (GUILayout.Button($"◂ {owners[i].name}", GUILayout.ExpandWidth(false))) {
					Selection.activeObject = owners[i];
				}
			}
			EditorGUILayout.EndHorizontal();
			DisabledField(OwnerProperty);

			EditorGUILayout.PropertyField(NameProperty);

			serializedObject.ApplyModifiedProperties();

			void DisabledField(SerializedProperty property, bool hasLabel = true) {
				EditorGUI.BeginDisabledGroup(true);
				if (hasLabel) {
					EditorGUILayout.PropertyField(property);
				} else {
					EditorGUILayout.PropertyField(property, GUIContent.none);
				}				
				EditorGUI.EndDisabledGroup();
			}

		}

		#endregion


		#region Private Properties

		private SerializedProperty ScriptProperty { get; set; }

		private SerializedProperty OwnerProperty { get; set; }

		private SerializedProperty NameProperty { get; set; }

		#endregion


	}

}