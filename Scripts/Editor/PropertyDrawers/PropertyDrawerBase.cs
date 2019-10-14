namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Utility class to make PropertyDrawers easier to implement.
	/// </summary>
	public class PropertyDrawerBase : PropertyDrawer {


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			float height = base.GetPropertyHeight(property, label);
			Property = property;
			Label = label;
			return height;
		}

		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			CurrentY = position.y;
			Position = position;
			Property = property;
			Label = label;
		}

		#endregion


		#region Protected Properties

		protected float CurrentY { get; private set; }

		protected Rect Position { get; private set; }

		protected SerializedProperty Property { get; private set; }

		protected GUIContent Label { get; private set; }

		protected float FieldHeight { get { return EditorGUIUtility.singleLineHeight + 2; } }

		#endregion


		#region Protected Methods

		/// <summary>
		/// Gets the next position for a IMGUI control.
		/// </summary>
		/// 
		/// <remarks>
		/// This returns rectangles from with standard field height and separation. 
		/// This can be called sequentially and it will return rectangles below one 
		/// another. It is reset at OnGUI().
		/// </remarks>
		/// 
		/// <returns>The next position.</returns>
		/// <param name="fieldsCount">How many fields should fit intor this rect.</param>
		/// <param name="preSpace">Vertical space before the rect.</param>
		/// <param name="postSpace">Vertical space after the rect.</param>
		protected Rect GetNextPosition(int fieldsCount = 1, float preSpace = 0, float postSpace = 0) {
			Rect rect = Position;
			CurrentY += preSpace;
			rect.y = CurrentY;
			rect.height = 0;
			for (int i = 0; i < fieldsCount; i++) {
				rect.height += FieldHeight;
			}
			rect.height -= 2;
			CurrentY += rect.height + 2;
			CurrentY += postSpace;
			return rect;
		}

		#endregion


	}

}