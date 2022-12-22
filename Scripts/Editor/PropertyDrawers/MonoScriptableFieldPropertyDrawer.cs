﻿namespace CocodriloDog.Core {

	using System;
    using System.Collections;
    using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Base class for property drawers used in subclasses of <see cref="MonoScriptableField{T}"/>.
	/// </summary>
	/// 
	/// <remarks>
	/// This drawer allows to create, select (edit) and remove <see cref="MonoScriptableObject"/>s
	/// that will be assigned to MonoBehaviours fields.
	/// </remarks>
	public abstract class MonoScriptableFieldPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);

			// Update pending stuff
			//
			// We need to defer these actions because the GenericMenu seems to be firing the actions
			// outside of the Update/ApplyModifiedproperties so the new MonoScriptableObject doesn't "stick"
			if (m_PendingMonoScriptableObject != null) {
				Property.serializedObject.FindProperty($"{m_PendingPathProperty}.m_Object").objectReferenceValue = m_PendingMonoScriptableObject;
				m_PendingMonoScriptableObject.name = m_PendingMonoScriptableObject.GetType().Name;
				m_PendingMonoScriptableObject.SetOwner(Property.serializedObject.targetObject);
				m_PendingMonoScriptableObject = null;
				m_PendingPathProperty = null;
			}

			// Vars
			var rect = GetNextPosition();
			var buttonsWidth = 110f;
			var monoScriptableObjectProperty = Property.FindPropertyRelative("m_Object");

			// The field
			Rect fieldRect = rect;
			fieldRect.xMax -= buttonsWidth;
			EditorGUI.BeginDisabledGroup(true);
			EditorGUIUtility.labelWidth -= buttonsWidth * 0.4f;
			EditorGUI.PropertyField(fieldRect, ObjectProperty, new GUIContent(Property.displayName));
			EditorGUIUtility.labelWidth = 0;
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

			// Check for array owner property
			var propertyPath = Property.propertyPath;
			var pathSteps = propertyPath.Split('.');

			//// This is an item of an array
			//if(pathSteps.Length >= 2 && pathSteps[pathSteps.Length - 2] == "Array") {
			//	var arrayPath = "";
			//	for(int i = 0; i < pathSteps.Length - 2; i++) {
			//		arrayPath += pathSteps[i];
			//	}
			//	var arrayProperty = Property.serializedObject.FindProperty(arrayPath);
			//	Debug.Log($"	arrayProperty: {arrayProperty.propertyPath}");
			//}

		}

		#endregion


		#region Protected Properties

		protected abstract List<Type> MonoScriptableTypes { get; }

		#endregion


		#region Protected Methods

		protected override void InitializePropertiesForOnGUI() {
			ObjectProperty = Property.FindPropertyRelative("m_Object");
		}

		#endregion


		#region Private Fields

		private MonoScriptableObject m_PendingMonoScriptableObject;

		private string m_PendingPathProperty;

		#endregion


		#region Private Properties

		private SerializedProperty ObjectProperty;

		#endregion


	}

}