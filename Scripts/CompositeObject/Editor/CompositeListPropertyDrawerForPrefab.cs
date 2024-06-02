namespace CocodriloDog.Core {

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
	/// data on components that can not be used anymore. For that reason, disabling the add, remove, create, delete functions
	/// from prefab instances will keep the components safer in terms of preseerving their healthy serialized data.
	/// </remarks>
	public class CompositeListPropertyDrawerForPrefab {


		#region Public Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="elements">The property of the list or array of composite objects.</param>
		/// <param name="isCompositeList">
		/// An optional flag that is needed when <see cref="CompositeList{T}"/> is used. In this case, 
		/// since the elements are two levels below the <see cref="CompositeList{T}"/> object, within the 
		/// <c>m_List</c>, we must use its parent property, the <see cref="CompositeList{T}"/> one.
		/// </param>
		public CompositeListPropertyDrawerForPrefab(SerializedProperty elements, bool isCompositeList = false) {
			m_SerializedObject = elements.serializedObject;
			m_IsCompositeList = isCompositeList;
			m_Lists[elements.propertyPath] = CreateList(elements.serializedObject, elements);
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

			GUIContent guiContent = null;

			// This is when CompositeList<> is used. It needs to bypass the m_List property upwards and get the
			// data from the parent property, the CompositeList<> itself.
			string parentPath = "";
			SerializedProperty parentProperty = null;

			if (m_IsCompositeList) {
				parentPath = elements.propertyPath.Remove(elements.propertyPath.LastIndexOf('.'));
				parentProperty = elements.serializedObject.FindProperty(parentPath);
			}
			
			if (m_IsCompositeList) {
				guiContent = new GUIContent(parentProperty.displayName, parentProperty.tooltip);
			} else {
				// This is when the Lis<CompositeObject> or CompositeObject[] is used
				guiContent = new GUIContent(elements.displayName, elements.tooltip);
			}

			elements.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(
				foldoutRect,
				elements.isExpanded,
				guiContent
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
						$"To add or remove {(m_IsCompositeList ? parentProperty.displayName : elements.displayName).ToLower()}, open the prefab.", 
						MessageType.Info
					);
				}

			}
		}

		public void DoLayoutList(SerializedProperty elements) {
			
			// Reserve layout space
			// It seems that there is no need to specify a width here
			GUILayoutUtility.GetRect(0, EditorGUI.GetPropertyHeight(elements));
			
			// Get the reserved rect
			var rect = GUILayoutUtility.GetLastRect();
			
			// Use it
			DoList(rect, elements);

		}

		#endregion


		#region Private Fields

		private Dictionary<string, ReorderableList> m_Lists = new Dictionary<string, ReorderableList>();

		private SerializedObject m_SerializedObject;

		private bool m_IsCompositeList;

		#endregion


		#region Private Properties

		// If it has no instance handle, it means that it is not governed by a prefab
		private bool CanAddRemove => PrefabUtility.GetPrefabInstanceHandle(m_SerializedObject.targetObject) == null;

		#endregion


		#region Private Methods

		private ReorderableList CreateList(SerializedObject serializedObject, SerializedProperty elements) {

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
