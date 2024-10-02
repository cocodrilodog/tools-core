namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif
	using System;
	using System.Reflection;
	using System.Linq;

	/// <summary>
	/// Base class for ScriptableObjects that may change at runtime, but have the option to
	/// recover their edit mode state when the Unity player stops.
	/// </summary> 
	public abstract class ScriptableResettable : ScriptableObject {


		#region Unity Methods

		protected virtual void OnEnable() {
#if UNITY_EDITOR
			if (!s_IsCloning && m_ResetOnEditMode) {
				EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
				// Create a clone as a backup for the data
				s_IsCloning = true;
				Clone = Instantiate(this);
				s_IsCloning = false;
			}
#endif
		}

		protected virtual void OnDisable() {
#if UNITY_EDITOR
			Clone = null;
			EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
#endif
		}

		#endregion


#if UNITY_EDITOR
		#region Event Handlers

		private void EditorApplication_playModeStateChanged(PlayModeStateChange state) {
			switch (state) {
				case PlayModeStateChange.ExitingPlayMode:
					// Recover the edit mode state from the backup
					CopyAllFields(m_Clone, this);
					Clone = null;
					break;
			}
		}


		#endregion


		#region Private Static Fields

		private static bool s_IsCloning;

		#endregion


		#region Private Fields - Serialized

		[Tooltip(
			"When checked, after Unity exits play mode, the values of this asset will " +
			"return to the ones before Unity entered play mode."
		)]
		[SerializeField]
		private bool m_ResetOnEditMode;

		#endregion


		#region Private Fields - Non Serialized

		[NonSerialized]
		private ScriptableResettable m_Clone;

		#endregion


		#region Private Properties

		private ScriptableResettable Clone {
			get => m_Clone;
			set {
				if (m_Clone != null) {
					if (Application.isPlaying) {
						Destroy(m_Clone);
					} else {
						DestroyImmediate(m_Clone);
					}
				}
				m_Clone = value;
			}
		}

		#endregion


		#region Private Methods

		/// <summary>
		/// Copies all fields, including inherited, from the <paramref name="source"/>
		/// into the <paramref name="destination"/>.
		/// </summary>
		/// <param name="source">The source ScriptableObject.</param>
		/// <param name="destination">The destination ScriptableObject.</param>
		private void CopyAllFields(ScriptableResettable source, ScriptableResettable destination) {
			Type type = GetType();
			while (type != null && type != typeof(ScriptableObject)) {
				CopyTypeFields(type,source, destination);
				type = type.BaseType;
			}
		}

		/// <summary>
		/// Copies the fields that belong to the <paramref name="type"/> from the <paramref name="source"/>
		/// into the <paramref name="destination"/>.
		/// </summary>
		/// <param name="type">The type to get the fields from.</param>
		/// <param name="source">The source ScriptableObject.</param>
		/// <param name="destination">The destination ScriptableObject.</param>
		private void CopyTypeFields(Type type, ScriptableResettable source, ScriptableResettable destination) {
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
							 .Where(field => field.IsPublic || field.GetCustomAttribute<SerializeField>() != null);
			foreach (var field in fields) {
				field.SetValue(destination, field.GetValue(source));
			}
		}

		#endregion
#endif


	}

}