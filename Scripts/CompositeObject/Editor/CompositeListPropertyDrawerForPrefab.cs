namespace CocodriloDog.Core.Editor {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEditorInternal;
	using UnityEngine;

	/// <summary>
	/// Special drawer that handles the drawing of an array or list of <see cref="CompositeObject"/>s 
	/// in such a way that it doesn't break when dealing with prefabs.
	/// </summary>
	/// 
	/// <remarks>
	/// When working with prefabs that have <see cref="CompositeObject"/> properties, certain changes in the prefab
	/// instances that are then applied to the prefab may break Unity's prefab serialization system, leading to corrupted 
	/// data on components that can no be used anymore. For that reason, disabling the add, remove, create, delete functions
	/// from prefab instances will keep the components safer in terms of preseerving their healthy serialized data.
	/// </remarks>
	public class CompositeListPropertyDrawerForPrefab {


		#region Public Constructor

		public CompositeListPropertyDrawerForPrefab(SerializedObject serializedObject, SerializedProperty elements, string label = null) {
			m_SerializedObject = serializedObject;
			m_Label = label;
			m_Lists[elements.propertyPath] = CreateList(serializedObject, elements);
		}

		#endregion


		#region Public Methods

		public void DoList(Rect rect, SerializedProperty elements) {

			// In a property drawer that is using this drawer for prefab, the intended property to be
			// drawn may change, or sometimes the property drawer will need to draw more than one property
			// per inspector refresh. For that reason we need to add as much lists as needed.
			if (!m_Lists.ContainsKey(elements.propertyPath)) {
				m_Lists[elements.propertyPath] = CreateList(elements.serializedObject, elements);
			}

			// Draw the foldout
			var foldoutRect = rect;
			foldoutRect.height = EditorGUIUtility.singleLineHeight + 2;
			foldoutRect.xMax -= 50;

			elements.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(
				foldoutRect,
				elements.isExpanded, 
				m_Label?.Length > 0 ? m_Label : elements.displayName
			);

			EditorGUI.EndFoldoutHeaderGroup();

			// Draw the size field
			var sizeRect = foldoutRect;
			sizeRect.x = foldoutRect.xMax + 2;
			sizeRect.width = 48;
			sizeRect.yMax -= 2;

			EditorGUI.BeginDisabledGroup(!CanAddRemove);
			elements.arraySize = EditorGUI.DelayedIntField(sizeRect, elements.arraySize);
			EditorGUI.EndDisabledGroup();

			// Draw the list
			var listRect = rect;
			listRect.yMin += foldoutRect.height;

			if (elements.isExpanded) {

				// Draw the list corresponding to the current property
				m_Lists[elements.propertyPath].DoList(listRect);

				// Draw prefab stuff
				var labelRect = rect;
				labelRect.yMin = labelRect.yMax;
				labelRect.yMin -= EditorGUIUtility.singleLineHeight + 2;

				if (!CanAddRemove) {
					EditorGUI.HelpBox(
						labelRect,
						$"To add or remove {(m_Label?.Length > 0 ? m_Label : elements.displayName).ToLower()}, open the prefab.", 
						MessageType.Info
					);
				}

			}
		}

		#endregion


		#region Private Fields

		private Dictionary<string, ReorderableList> m_Lists = new Dictionary<string, ReorderableList>();

		private SerializedObject m_SerializedObject;

		private string m_Label;

		#endregion


		#region Private Properties

		// If it has no instance handle, it means that it is not governed by a prefab
		private bool CanAddRemove => PrefabUtility.GetPrefabInstanceHandle(m_SerializedObject.targetObject) == null;

		#endregion


		#region Private Methods

		public ReorderableList CreateList(SerializedObject serializedObject, SerializedProperty elements) {

			var propertyType = CDEditorUtility.GetPropertyType(elements);
			bool isArrayOrListOfCompositeObject = false;

			if (propertyType.IsArray && typeof(CompositeObject).IsAssignableFrom(typeof(CompositeObject))) {
				isArrayOrListOfCompositeObject = true;
			}

			if (propertyType.IsGenericType &&
				propertyType.GetGenericTypeDefinition() == typeof(List<>) &&
				typeof(CompositeObject).IsAssignableFrom(propertyType.GetGenericArguments()[0])) {
				isArrayOrListOfCompositeObject = true;
			}

			if (!isArrayOrListOfCompositeObject) {
				throw new ArgumentException($"The provided {nameof(elements)} must correspond to an array or list of CompositeObject");
			}

			var canAddRemove = CanAddRemove;
			var list = new ReorderableList(serializedObject, elements, true, false, canAddRemove, canAddRemove);
			list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				EditorGUI.PropertyField(rect, elements.GetArrayElementAtIndex(index));
			};
			list.elementHeightCallback = (int index) => {
				return EditorGUI.GetPropertyHeight(elements.GetArrayElementAtIndex(index));
			};

			return list;

		}

		#endregion


	}

}
