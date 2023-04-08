namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ObjectFinder))]
	public class ObjectScriptablePropertyDrawer : PropertyDrawerBase {


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var height = base.GetPropertyHeight(property, label);
			height += EditorGUI.GetPropertyHeight(ObjectProperty);
			height += FieldHeight;
			height += EditorGUI.GetPropertyHeight(PathProperty);
			return height;
		}

		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			base.OnGUI(position, property, label);
			label = EditorGUI.BeginProperty(Position, Label, Property);
			EditorGUI.PropertyField(GetNextPosition(ObjectProperty), ObjectProperty);
			DrawObjectScriptable();
			EditorGUI.PropertyField(GetNextPosition(PathProperty), PathProperty);
			EditorGUI.EndProperty();
		}

		#endregion


		#region Protected Methods

		protected override void InitializePropertiesForGetHeight() {
			base.InitializePropertiesForGetHeight();
			ObjectProperty = Property.FindPropertyRelative("m_Object");
			PathProperty = Property.FindPropertyRelative("m_Path");
		}

		#endregion


		#region Private Properties

		private SerializedProperty ObjectProperty { get; set; }

		private SerializedProperty PathProperty { get; set; }

		#endregion

		private Object m_Obj;

		#region Private Methods

		private void DrawObjectScriptable() {

			var rect = GetNextPosition();

			var labelRect = rect;
			labelRect.width = EditorGUIUtility.labelWidth;
			//EditorGUI.LabelField(labelRect, "Object S!");

			var objectRect = rect;
			objectRect.xMin += EditorGUIUtility.labelWidth + 2;
			//GUI.Box(objectRect, new GUIContent(ObjectProperty.displayName), EditorStyles.objectField);
			EditorGUI.PropertyField(rect, ObjectProperty);
			//EditorGUI.ObjectField(rect, m_Obj, typeof(Object), true);

			if (rect.Contains(Event.current.mousePosition)) {
				if (Event.current.type == EventType.DragUpdated) {
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					Debug.Log("Drag Updated!");
					Event.current.Use();
				}
				if (Event.current.type == EventType.DragPerform) {
					DragAndDrop.AcceptDrag();
					//foreach (Object draggedObject in DragAndDrop.objectReferences) {

					//}
					ObjectProperty.objectReferenceValue = DragAndDrop.objectReferences[0];
					Debug.Log(DragAndDrop.objectReferences[0]);
					Debug.Log(ObjectProperty.objectReferenceValue);
					PathProperty.stringValue = DragAndDrop.objectReferences[0].name;
					//DragAndDrop.visualMode = DragAndDropVisualMode.;
					Debug.Log("Drag DragPerform!");
					Event.current.Use();
					m_Obj = DragAndDrop.objectReferences[0];
				}
			}

		}

		#endregion


	}

}