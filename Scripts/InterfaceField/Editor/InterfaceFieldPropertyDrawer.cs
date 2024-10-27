namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEditor.Experimental.GraphView;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(InterfaceField<>), true)]
	public class InterfaceFieldPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);

			EditorGUI.BeginProperty(Position, Label, Property);
			
			// Get the type of the property
			var propertyType = CDEditorUtility.GetPropertyType(Property);
			
			// Get the InterfaceField<T> type and handle if the property is an array or list
			var fieldType = SystemUtility.IsArrayOrList(propertyType) ? SystemUtility.GetElementType(propertyType) : propertyType;
			
			// Get the interface type
			var interfaceType = fieldType.GetGenericArguments()[0];
			if (interfaceType.IsInterface) {

				// Draw the label and get the rect excluding the label
				var fieldRect = EditorGUI.PrefixLabel(Position, GUIUtility.GetControlID(FocusType.Passive), Label);

				// TODO: Override the picker functionality (Currently there are no options displayed in the window)
				PickObject(fieldRect, interfaceType);

				// Handle drag and drop (This overrides the default behaviour)
				//
				// By setting the type of the ObjectField to interfaceType, when the field is empty, it shows the type of the
				// interface. That is great, but it won't receive game objects, which makes it very hard to use.
				//
				// For that reason, while keeping the interfaceType in the object field, we can override the drag and drop
				// behaviour so that it receives game objects that have interfaceType components attached to it.
				DragAndDrop(fieldRect, interfaceType);
				
				// Draw the field
				DrawField(fieldRect, interfaceType);

				// Draw a blue overlay to show it as a valid target (This is a bit too hacky, but it looks good enough!)
				DrawBlueOverlay(fieldRect);

			} else {
				Debug.LogError($"Type {propertyType} must use an interface in the generic parameter.");
			}

			EditorGUI.EndProperty();

		}

		#endregion


		#region Private Fields

		private bool m_IsDraggingValidObject;

		/// <summary>
		/// This is used to keep track of the dragged object, if it is part of an array or list.
		/// If this index coincides with the field under the cursor, the blue overlay is drawn. 
		/// </summary>
		/// <remarks>
		/// This prevents all the elements of the list to be highlighted concurrently when a dragged 
		/// object approaches one of the fields in the list.
		/// </remarks>
		private int m_DraggingElementIndex = -1;

		#endregion


		#region Private Methods

		private void PickObject(Rect fieldRect, Type interfaceType) {

			var pickerRect = fieldRect;
			pickerRect.xMin = pickerRect.xMax - 20;

			// Check if the field was clicked
			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && pickerRect.Contains(Event.current.mousePosition)) {
			
				Event.current.Use();

				// https://www.youtube.com/watch?v=0HHeIUGsuW8
				var context = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));

				var valueProperty = Property.FindPropertyRelative("m_Value");
				var optionsProvider = new InterfaceOptionsProvider(interfaceType, o => {
					Property.serializedObject.Update();
					valueProperty.objectReferenceValue = o;
					Property.serializedObject.ApplyModifiedProperties();
				});

				SearchWindow.Open(context, optionsProvider);
			}

		}

		private void DragAndDrop(Rect fieldRect, Type interfaceType) {
			
			Event evt = Event.current;

			switch (evt.type) {
				case EventType.DragUpdated:
				case EventType.DragPerform:

					// Validate the area
					if (!fieldRect.Contains(evt.mousePosition) || UnityEditor.DragAndDrop.objectReferences.Length > 1) {
						m_IsDraggingValidObject = false;
						m_DraggingElementIndex = -1;
						return;
					}

					// Get the only dragged object
					UnityEngine.Object draggedObject = UnityEditor.DragAndDrop.objectReferences[0];

					// Search for the interface implementation in the dragged object
					if (draggedObject is GameObject go) {
						// GameObject
						draggedObject = go.GetComponent(interfaceType);
					} else {
						// Component or ScriptableObject
						if (draggedObject != null) {
							draggedObject = interfaceType.IsAssignableFrom(draggedObject.GetType()) ? draggedObject : null;
						}
					}

					// Accept the drag operation
					if (draggedObject != null && interfaceType.IsAssignableFrom(draggedObject.GetType())) {
						// The drag is valid
						UnityEditor.DragAndDrop.visualMode = DragAndDropVisualMode.Link;
						m_IsDraggingValidObject = true;
						if (CDEditorUtility.IsArrayElement(Property)) {
							m_DraggingElementIndex = CDEditorUtility.GetElementIndex(Property);
						} else {
							m_DraggingElementIndex = -1;
						}
						// Accept the drag
						if (evt.type == EventType.DragPerform) {
							UnityEditor.DragAndDrop.AcceptDrag();
							var valueProperty = Property.FindPropertyRelative("m_Value");
							valueProperty.objectReferenceValue = draggedObject;
							m_IsDraggingValidObject = false;
							m_DraggingElementIndex = -1;
						}
						evt.Use();
					} else {
						// Reject the drag operation
						UnityEditor.DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
						m_IsDraggingValidObject = false;
						m_DraggingElementIndex = -1;
					}

					break;
			}

		}

		private void DrawField(Rect fieldRect, Type interfaceType) {
			var valueProperty = Property.FindPropertyRelative("m_Value");
			EditorGUI.BeginChangeCheck();
			var value = EditorGUI.ObjectField(fieldRect, valueProperty.objectReferenceValue, interfaceType, true);
			if (EditorGUI.EndChangeCheck()) {
				valueProperty.objectReferenceValue = value;
			}
		}

		private void DrawBlueOverlay(Rect fieldRect) {

			//TODO: In the example scene, the last field is left blue some times.
			var color = new Color(0.2f, 0.65f, 1.0f, 0.4f);
			var boxRect = fieldRect;
			//boxRect.xMin += 1;
			boxRect.xMax -= 20; // <- This is the only modification that looks good. I left the others commented for reference
			//boxRect.yMin += 1;
			//boxRect.yMax -= 2;

			if (m_IsDraggingValidObject) {
				if (CDEditorUtility.IsArrayElement(Property)) {
					if (CDEditorUtility.GetElementIndex(Property) == m_DraggingElementIndex) {
						EditorGUI.DrawRect(boxRect, color);
					}
				} else {
					EditorGUI.DrawRect(boxRect, color);
				}
			}

		}

		#endregion


	}

}