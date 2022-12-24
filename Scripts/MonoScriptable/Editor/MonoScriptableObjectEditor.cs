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
			DisabledField(OwnerProperty);
			if (GUILayout.Button("Inspect", GUILayout.Width(60))) {
				Selection.activeObject = (target as MonoScriptableObject).Owner;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.PropertyField(NameProperty);

			serializedObject.ApplyModifiedProperties();

			void DisabledField(SerializedProperty property) {
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.PropertyField(property);
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