namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Utility class to make PropertyDrawers easier to implement.
	/// </summary>
	public class PropertyDrawerBase : PropertyDrawer {


		#region Public Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			float height = base.GetPropertyHeight(property, label);
			Property = property;
			Label = label;
			InitializePropertiesForGetHeight();
			return height;
		}

		#endregion


		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			CurrentY = position.y;
			Position = position;
			Property = property;
			Label = label;
			InitializePropertiesForOnGUI();
		}

		#endregion


		#region Protected Properties

		protected Rect Position { get; private set; }

		protected SerializedProperty Property {
			get { return m_Property; }
			private set { 
				m_Property = value;
				SerializedObject = m_Property.serializedObject;
			}
		}

		protected GUIContent Label { get; set; }

		protected float FieldHeight => EditorGUIUtility.singleLineHeight + 2;

		protected float SpaceHeight => 10;

		#endregion


		#region Protected Methods

		/// <summary>
		/// Gets the next position (<c>Rect</c>) for a IMGUI control.
		/// </summary>
		/// 
		/// <remarks>
		/// This returns rectangles with standard field height and reserves 2 pixels of separation 
		/// before the next one. This can be called sequentially and it will return rectangles below one 
		/// another. It is reset at OnGUI().
		/// </remarks>
		/// 
		/// <returns>The next position.</returns>
		/// <param name="fieldsCount">How many fields should fit into this rect.</param>
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

		/// <summary>
		/// Gets the next position (<c>Rect</c>) for a IMGUI control with the specified 
		/// <paramref name="height"/> and doesn't reserve any separation before the next one.
		/// </summary>
		/// 
		/// <remarks>
		/// This can be called sequentially and it will return rectangles below one 
		/// another. It is reset at OnGUI().
		/// </remarks>
		/// 
		/// <param name="height">The height</param>
		/// <returns>The next position</returns>
		protected Rect GetNextPosition(float height) {
			Rect rect = Position;
			rect.y = CurrentY;
			rect.height = height;
			CurrentY += rect.height;
			return rect;
		}

		/// <summary>
		/// Gets the next position (<c>Rect</c>) for a IMGUI control with the height for the 
		/// provided <paramref name="property"/> and reserves 2 extra pixels of separation
		/// before the next one.
		/// </summary>
		/// 
		/// <remarks>
		/// This can be called sequentially and it will return rectangles below one 
		/// another. It is reset at OnGUI().
		/// </remarks>
		/// 
		/// <param name="property">The property</param>
		/// <returns></returns>
		protected Rect GetNextPosition(SerializedProperty property) {
			Rect rect = Position;
			rect.y = CurrentY;
			rect.height = EditorGUI.GetPropertyHeight(property);
			CurrentY += rect.height;
			CurrentY += 2;
			return rect;
		}

		/// <summary>
		/// Called when the serialized object of the property changes.
		/// </summary>
		/// 
		/// <remarks>
		/// This may happen when selecting a different instance of the same type, 
		/// both inspected by the same editor. It is a good time to renew any cached
		/// serialized data.
		/// </remarks>
		protected virtual void OnSerializedObjectChange() { }

		/// <summary>
		/// Called after setting <see cref="Property"/> and <see cref="Label"/>
		/// on <see cref=" GetPropertyHeight(SerializedProperty, GUIContent)"/>
		/// </summary>
		/// 
		/// <remarks>
		/// It is a good place to initialize properties that should be available
		/// for the OnGUI cycle and that are already required to calculate the height
		/// of the property.
		/// </remarks>
		protected virtual void InitializePropertiesForGetHeight() { }

		/// <summary>
		/// Called after setting <see cref="Position"/>, <see cref="Property"/> and 
		/// <see cref="Label"/> on <see cref="OnGUI(Rect, SerializedProperty, GUIContent)"/>
		/// </summary>
		/// 
		/// <remarks>
		/// It is a good place to initialize properties that should be available
		/// for the OnGUI cycle.
		/// </remarks>
		protected virtual void InitializePropertiesForOnGUI() { }

		#endregion


		#region Private Fields

		private SerializedProperty m_Property;

		private SerializedObject m_SerializedObject;

		#endregion


		#region Private properties

		private float CurrentY { get; set; }

		private SerializedObject SerializedObject {
			get { return m_SerializedObject; }
			set {
				if (value != m_SerializedObject) {
					m_SerializedObject = value;
					OnSerializedObjectChange();
				}
			}
		}

		#endregion


	}

}