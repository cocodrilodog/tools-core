namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
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

				DropAreaGUI(Position, interfaceType);

				// Update the field, in case the value changed by an object picker action
				var valueProperty = Property.FindPropertyRelative("m_Value");
				valueProperty.objectReferenceValue = EditorGUI.ObjectField(position, label, valueProperty.objectReferenceValue, interfaceType, true);
			
			} else {
				Debug.LogError($"Type {propertyType} must use an interface in the generic parameter.");
			}

			EditorGUI.EndProperty();

		}

		#endregion


		#region Private Methods

		private void DropAreaGUI(Rect dropArea, Type interfaceType) {
			
			Event evt = Event.current;

			switch (evt.type) {
				case EventType.DragUpdated:
				case EventType.DragPerform:

					// Validate the area
					if (!dropArea.Contains(evt.mousePosition) || DragAndDrop.objectReferences.Length > 1) {
						return;
					}

					// Get the only dragged object
					UnityEngine.Object draggedObject = DragAndDrop.objectReferences[0];

					// Search for the interface implementation in the dragged object
					if (draggedObject is GameObject go) {
						// GameObject
						draggedObject = go.GetComponent(interfaceType);
					} else {
						// Component
						if (draggedObject != null) {
							draggedObject = interfaceType.IsAssignableFrom(draggedObject.GetType()) ? draggedObject : null;
						}
					}

					// Reject the drag operation
					if (draggedObject == null || !interfaceType.IsAssignableFrom(draggedObject.GetType())) {
						DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
						return;
					}

					// The drag is valid
					DragAndDrop.visualMode = DragAndDropVisualMode.Link;

					// Accept the drag
					if (evt.type == EventType.DragPerform) {
						DragAndDrop.AcceptDrag();
						var valueProperty = Property.FindPropertyRelative("m_Value");
						valueProperty.objectReferenceValue = draggedObject;
					}

					evt.Use();

					break;
			}

		}

		#endregion


	}

}