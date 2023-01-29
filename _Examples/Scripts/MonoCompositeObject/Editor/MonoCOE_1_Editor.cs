namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Example editor for <see cref="MonoCOE_1"/>.
	/// </summary>
	/// <remarks>
	/// Adds the properties specific to <see cref="MonoCOE_1"/>
	/// </remarks>
	[CustomEditor(typeof(MonoCOE_1))]
    public class MonoCOE_1_Editor : MonoCompositeObjectEditor {


		#region Unity Methods

		protected override void OnEnable() {
			base.OnEnable();
			if (target != null) {
				Prop1Property = serializedObject.FindProperty("Prop1");
			}
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