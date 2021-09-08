namespace CocodriloDog.Core.Examples {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

	/// <summary>
	/// Example editor for <see cref="MonoSOE_1"/>.
	/// </summary>
	/// <remarks>
	/// Adds the properties specific to <see cref="MonoSOE_1"/>
	/// </remarks>
	[CustomEditor(typeof(MonoSOE_1))]
    public class MonoSOE_1_Editor : MonoScriptableObjectEditor {


		#region Unity Methods

		protected override void OnEnable() {
			base.OnEnable();
			Prop1Property = serializedObject.FindProperty("Prop1");
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			serializedObject.Update();
			EditorGUILayout.PropertyField(Prop1Property);
			serializedObject.ApplyModifiedProperties();
		}

		#endregion


		#region Private Properties

		private SerializedProperty Prop1Property { get; set; }

		#endregion


	}

}