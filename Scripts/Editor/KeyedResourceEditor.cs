namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditorInternal;

	public abstract class KeyedResourceEditor : Editor {


		#region Unity Methods

		public override void OnInspectorGUI() {
			serializedObject.Update();
			DrawFallback();
			KeyedResourceList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}

		#endregion


		#region Protected Properties

		protected virtual float PropertiesMiddle { get { return 0.5f; } }

		protected virtual string KeyLabel { get { return "Key"; } }

		protected virtual float KeyLabelWidth { get { return 30; } }

		protected virtual string ResourceLabel { get { return "Resource"; } }

		protected virtual float ResourceLabelWidth { get { return 60; } }

		protected abstract string FallbackKeyValue { get; }

		protected SerializedProperty KeyedResourceListProperty {
			get { return m_KeyedResourceListProperty = m_KeyedResourceListProperty ?? serializedObject.FindProperty("m_KeyedResourceList"); }
		}

		#endregion


		#region Protected Methods

		protected virtual void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused) {

			SerializedProperty elementProperty = KeyedResourceListProperty.GetArrayElementAtIndex(index);

			EditorGUIUtility.labelWidth = KeyLabelWidth;
			Rect keyRect = rect;
			keyRect.width = keyRect.width * PropertiesMiddle - 2;
			keyRect.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(keyRect, elementProperty.FindPropertyRelative("Key"), new GUIContent(KeyLabel));

			EditorGUIUtility.labelWidth = ResourceLabelWidth;
			Rect resourceRect = rect;
			resourceRect.x += resourceRect.width * PropertiesMiddle + 2;
			resourceRect.width = (resourceRect.width * (1 - PropertiesMiddle)) - 2;
			resourceRect.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(resourceRect, elementProperty.FindPropertyRelative("Resource"), new GUIContent(ResourceLabel));

			EditorGUIUtility.labelWidth = 0;

		}

		protected virtual float ElementHeightCallback(int index) {
			return (EditorGUIUtility.singleLineHeight + 2) + 2;
		}

		#endregion


		#region Private Fields

		private ReorderableList m_KeyedResourceList;

		private SerializedProperty m_KeyedResourceListProperty;

		#endregion


		#region Private Properties

		private ReorderableList KeyedResourceList {
			get {
				if(m_KeyedResourceList == null) {
					m_KeyedResourceList = new ReorderableList(
						serializedObject, KeyedResourceListProperty, true, true, true, true
					);
					m_KeyedResourceList.drawElementCallback = KeyedResourceList_drawElementCallback;
					m_KeyedResourceList.elementHeightCallback = KeyedResourceList_elementHeightCallback;
					m_KeyedResourceList.drawHeaderCallback = KeyedResourceList_drawHeaderCallback;
				}
				return m_KeyedResourceList;
			}
		}

		#endregion


		#region Private Methods

		void KeyedResourceList_drawHeaderCallback(Rect rect) {
			EditorGUI.LabelField(
				rect,
				ObjectNames.NicifyVariableName(serializedObject.targetObject.GetType().Name)
			);
		}

		private void KeyedResourceList_drawElementCallback(Rect rect, int index, bool isActive, bool isFocused) {
			DrawElementCallback(rect, index, isActive, isFocused);
		}

		private float KeyedResourceList_elementHeightCallback(int index) {
			return ElementHeightCallback(index);
		}

		private void DrawFallback() {
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.Space();
			EditorGUILayout.Popup("Fallback", 0, new string[1] { FallbackKeyValue });
			EditorGUILayout.Space();
			EditorGUI.EndDisabledGroup();
		}

		#endregion


	}

}