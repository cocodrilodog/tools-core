namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(CompositeRoot))]
	public class CompositeRootEditor : Editor {


		#region Unity Methods

		protected virtual void OnEnable() {
			SelectedCompositePathProperty = serializedObject.FindProperty("m_SelectedCompositePath");
			//Debug.Log($"SelectedCompositePathProperty: {SelectedCompositePathProperty}");
		}

		public sealed override void OnInspectorGUI() {
			if (string.IsNullOrEmpty(SelectedCompositePathProperty.stringValue)) {
				OnRootInspectorGUI();
			} else {
				serializedObject.Update();
				var selectedProperty = serializedObject.FindProperty(SelectedCompositePathProperty.stringValue);
				if (selectedProperty != null) {
					EditorGUILayout.PropertyField(selectedProperty);
				}
				//Debug.Log($"PATH {SelectedCompositePathProperty.stringValue}");
				serializedObject.ApplyModifiedProperties();
			}
		}

		#endregion


		#region Protected Methods

		public virtual void OnRootInspectorGUI() => base.OnInspectorGUI();

		#endregion


		#region Private Properties

		private SerializedProperty SelectedCompositePathProperty { get; set; }

		#endregion


	}

}