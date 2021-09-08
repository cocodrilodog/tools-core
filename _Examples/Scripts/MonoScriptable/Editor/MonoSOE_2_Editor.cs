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

		protected override void OnEnable() {
			base.OnEnable();
			Prop2Property = serializedObject.FindProperty("Prop2");
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			serializedObject.Update();
			EditorGUILayout.PropertyField(Prop2Property);
			serializedObject.ApplyModifiedProperties();
		}

		#endregion


		#region Private Properties

		private SerializedProperty Prop2Property { get; set; }

		#endregion


	}

}