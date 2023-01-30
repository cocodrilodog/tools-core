namespace CocodriloDog.Core {

	using CocodriloDog.Core.Examples;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Base class for editors of <see cref="MonoCompositeRoot"/> objects.
	/// </summary>
	/// 
	/// <remarks>
	/// Replaces the intended editor of the <see cref="MonoCompositeRoot"/> with the one 
	/// of the <see cref="MonoCompositeRoot.SelectedMonoCompositeObject"/> while there is one selected.
	/// Otherwise invokes <see cref="OnRootInspectorGUI"/> for subclasses to create their own inspectors
	/// when there is no <see cref="MonoCompositeRoot.SelectedMonoCompositeObject"/>.
	/// </remarks>
	public class MonoCompositeRootEditor : Editor {


		#region Unity Methods

		protected virtual void OnEnable() {
			SelectedMonoCompositeObjectProperty = serializedObject.FindProperty("m_SelectedMonoCompositeObject");
		}

		public sealed override void OnInspectorGUI() {
			if (SelectedMonoCompositeObjectProperty.objectReferenceValue != null) {
				Editor.CreateCachedEditor(SelectedMonoCompositeObjectProperty.objectReferenceValue, null, ref m_PreviousEditor);
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

		private SerializedProperty SelectedMonoCompositeObjectProperty { get; set; }

		#endregion


	}

}