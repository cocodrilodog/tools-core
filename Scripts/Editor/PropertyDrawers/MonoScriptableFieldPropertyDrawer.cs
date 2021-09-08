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

			// Update pending stuff
			//
			// We need to defer these actions because the GenericMenu seems to be firing the actions
			// outside of the Update/ApplyModifiedproperties so the new MonoScriptableObject doesn't "stick"
			if (m_PendingMonoScriptableObject != null) {
				Property.serializedObject.FindProperty($"{m_PendingPathProperty}.m_MonoScriptableObject").objectReferenceValue = m_PendingMonoScriptableObject;
				m_PendingMonoScriptableObject.name = m_PendingMonoScriptableObject.GetType().Name;
				m_PendingMonoScriptableObject.SetOwner(Property.serializedObject.targetObject);
				m_PendingMonoScriptableObject = null;
				m_PendingPathProperty = null;
			}

			// Vars
			var rect = GetNextPosition();
			var buttonsWidth = 110f;
			var monoScriptableObjectProperty = Property.FindPropertyRelative("m_MonoScriptableObject");

			// The field
			Rect fieldRect = rect;
			fieldRect.xMax -= buttonsWidth;
			EditorGUI.BeginDisabledGroup(true);
			EditorGUI.PropertyField(fieldRect, MonoScriptableObjectProperty, new GUIContent(Property.displayName));
			EditorGUI.EndDisabledGroup();

			// Create/Edit button
			Rect createEditRect = rect;
			createEditRect.xMin += rect.width - buttonsWidth;
			createEditRect.xMax -= buttonsWidth * 0.53f;

			if (monoScriptableObjectProperty.objectReferenceValue == null) {
				if (GUI.Button(createEditRect, "Create")) {

					// Save the path because when the object is an array element, it will need the path with the index
					// in order to assign the value to the correct slot in the deferred assignment above. Otherwise it 
					// will always assign the value to the first slot.
					m_PendingPathProperty = Property.propertyPath;

					// Show the menu only when there are more than one types
					if (MonoScriptableTypes.Count > 1) {
						var menu = new GenericMenu();
						foreach (var type in MonoScriptableTypes) {
							menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(type.Name)), false, () => {
								m_PendingMonoScriptableObject = ScriptableObject.CreateInstance(type) as MonoScriptableObject;
							});
						}
						menu.ShowAsContext();
					} else {
						m_PendingMonoScriptableObject = ScriptableObject.CreateInstance(MonoScriptableTypes[0]) as MonoScriptableObject;
					}

				}
			} else {
				if (GUI.Button(createEditRect, "Edit")) {
					Selection.activeObject = monoScriptableObjectProperty.objectReferenceValue;
				}
			}

			// Remove button
			Rect removeRect = rect;
			removeRect.xMin += rect.width - buttonsWidth;
			removeRect.xMin += buttonsWidth * 0.47f; 
			if(GUI.Button(removeRect, "Remove")) {
				monoScriptableObjectProperty.objectReferenceValue = null;				
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

		private MonoScriptableObject m_PendingMonoScriptableObject;

		private string m_PendingPathProperty;

		#endregion


		#region Private Properties

		private SerializedProperty MonoScriptableObjectProperty;

		#endregion


	}

}