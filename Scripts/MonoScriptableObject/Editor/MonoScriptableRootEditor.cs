namespace CocodriloDog.Core {

	using CocodriloDog.Core.Examples;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Base class for editors of <see cref="MonoScriptableRoot"/> objects.
	/// </summary>
	/// 
	/// <remarks>
	/// Replaces the intended editor of the <see cref="MonoScriptableRoot"/> with the one 
	/// of the <see cref="MonoScriptableRoot.SelectedMonoScriptableObject"/> while there is one selected.
	/// Otherwise invokes <see cref="OnRootInspectorGUI"/> for subclasses to create their own inspectors
	/// when there is no <see cref="MonoScriptableRoot.SelectedMonoScriptableObject"/>.
	/// </remarks>
	public class MonoScriptableRootEditor : Editor {


		#region Unity Methods

		private void OnEnable() {
			SelectedMonoScriptableObjectProperty = serializedObject.FindProperty("m_SelectedMonoScriptableObject");
		}

		public sealed override void OnInspectorGUI() {
			if (SelectedMonoScriptableObjectProperty.objectReferenceValue != null) {
				Editor.CreateCachedEditor(SelectedMonoScriptableObjectProperty.objectReferenceValue, null, ref m_PreviousEditor);
				m_PreviousEditor.OnInspectorGUI();
			} else {
				OnRootInspectorGUI();
			}
		}

		#endregion


		#region Protected Methods

		public virtual void OnRootInspectorGUI() => base.OnInspectorGUI();

		#endregion


		#region Private Fields

		private Editor m_PreviousEditor;

		#endregion


		#region Private Properties

		private SerializedProperty SelectedMonoScriptableObjectProperty { get; set; }

		#endregion


	}

}