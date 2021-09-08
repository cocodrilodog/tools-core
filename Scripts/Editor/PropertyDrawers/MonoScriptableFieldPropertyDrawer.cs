namespace CocodriloDog.Core {

	using System;
    using System.Collections;
    using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

    public abstract class MonoScriptableFieldPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);
			
			Rect rect = GetNextPosition();
			float buttonsWidth = 110;
			
			// The field
			Rect fieldRect = rect;
			fieldRect.xMax -= buttonsWidth;
			EditorGUI.BeginDisabledGroup(true);
			EditorGUI.PropertyField(fieldRect, MonoScriptableObjectProperty, new GUIContent(Property.displayName));
			EditorGUI.EndDisabledGroup();

			// Create/Edit button
			Rect createButtonRect = rect;
			createButtonRect.xMin += rect.width - buttonsWidth;
			createButtonRect.xMax -= buttonsWidth * 0.53f;
			if(GUI.Button(createButtonRect, "Create")) {
				var menu = new GenericMenu();
				foreach(var type in MonoScriptableTypes) {
					menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(type.Name)), false, () => {
						m_NewMonoScriptableObject = ScriptableObject.CreateInstance(type) as MonoScriptableObject;
					});
				}
				menu.ShowAsContext();
			}

			// Remove button
			Rect removeButtonRect = rect;
			removeButtonRect.xMin += rect.width - buttonsWidth;
			removeButtonRect.xMin += buttonsWidth * 0.47f;
			if(GUI.Button(removeButtonRect, "Remove")) {
				Property.FindPropertyRelative("m_MonoScriptableObject").objectReferenceValue = null;
			}

			// Deferred actions
			if (m_NewMonoScriptableObject != null) {
				Property.FindPropertyRelative("m_MonoScriptableObject").objectReferenceValue = m_NewMonoScriptableObject;
				m_NewMonoScriptableObject = null;
			}

		}

		#endregion


		#region Protected Properties

		protected abstract List<Type> MonoScriptableTypes { get; }

		#endregion


		#region Protected Methods

		protected override void InitializePropertiesForOnGUI() {
			MonoScriptableObjectProperty = Property.FindPropertyRelative("m_MonoScriptableObject");
		}

		#endregion


		#region Private Fields

		private MonoScriptableObject m_NewMonoScriptableObject;

		#endregion


		#region Private Properties

		private SerializedProperty MonoScriptableObjectProperty;

		#endregion


	}

}