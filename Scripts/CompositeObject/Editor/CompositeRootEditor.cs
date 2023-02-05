namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Base class for concrete implementations of <see cref="CompositeRoot"/>.
	/// </summary>
	public abstract class CompositeRootEditor : Editor {


		#region Unity Methods

		protected virtual void OnEnable() {
			ScriptProperty = serializedObject.FindProperty("m_Script");
			SelectedCompositePathProperty = serializedObject.FindProperty("m_SelectedCompositePath");
		}

		public sealed override void OnInspectorGUI() {
			if (string.IsNullOrEmpty(SelectedCompositePathProperty.stringValue)) {
				// There is no selected composite object, proceed with the inspector of the root object
				OnRootInspectorGUI();
			} else {
				// There is a selected composite object, draw only it as a property.
				serializedObject.Update();
				CDEditorUtility.DrawDisabledField(ScriptProperty);
				var selectedCompositeProperty = serializedObject.FindProperty(SelectedCompositePathProperty.stringValue);
				if (selectedCompositeProperty != null) {
					EditorGUILayout.PropertyField(selectedCompositeProperty);
				}
				serializedObject.ApplyModifiedProperties();
			}
		}

		#endregion


		#region Protected Methods

		/// <summary>
		/// In subclasses, override this instead of <see cref="OnInspectorGUI"/>.
		/// </summary>
		/// <remarks>
		/// This editor will choose to display this GUI when no <see cref="CompositeObject"/> is selected.
		/// Otherwise it will render the property drawer of the selected <see cref="CompositeObject"/>.
		/// </remarks>
		protected virtual void OnRootInspectorGUI() => base.OnInspectorGUI();

		#endregion


		#region Private Properties

		private SerializedProperty ScriptProperty { get; set; }

		private SerializedProperty SelectedCompositePathProperty { get; set; }

		#endregion


	}

}