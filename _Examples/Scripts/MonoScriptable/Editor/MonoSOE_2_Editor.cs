namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Example editor for <see cref="MonoSOE_2"/>.
	/// </summary>
	/// <remarks>
	/// Adds the properties specific to <see cref="MonoSOE_2"/>
	/// </remarks>
	[CustomEditor(typeof(MonoSOE_2))]
	public class MonoSOE_2_Editor : MonoScriptableObjectEditor {


		#region Unity Methods

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			serializedObject.Update();
			foreach(var property in Properties) {
				EditorGUILayout.PropertyField(property);
			}
			serializedObject.ApplyModifiedProperties();
		}

		#endregion


		#region Private Fields

		private List<SerializedProperty> m_Properties;

		#endregion


		#region Private Properties

		private List<SerializedProperty> Properties {
			get {
				if(m_Properties == null) {
					m_Properties = new List<SerializedProperty> {
						serializedObject.FindProperty("Prop2"),
						serializedObject.FindProperty("m_ChildMonoSOE")
					};
				}
				return m_Properties;
			}
		}

		#endregion


	}

}