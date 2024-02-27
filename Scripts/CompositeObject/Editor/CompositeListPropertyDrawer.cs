namespace CocodriloDog.Core.Editor {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(CompositeList<>), true)]
	public class CompositeListPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var height = base.GetPropertyHeight(property, label);
			var listProperty = Property.FindPropertyRelative("m_List");
			return listProperty != null ? EditorGUI.GetPropertyHeight(listProperty) : height;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			base.OnGUI(position, property, label);
			var listProperty = Property.FindPropertyRelative("m_List");
			if(listProperty != null) {
				ListDrawer.DoList(GetNextPosition(listProperty), listProperty);
			}
		}

		#endregion


		#region Private Fields

		private CompositeListPropertyDrawerForPrefab m_ListDrawer;

		#endregion


		#region Private Properties

		private CompositeListPropertyDrawerForPrefab ListDrawer => 
			m_ListDrawer = m_ListDrawer ?? new CompositeListPropertyDrawerForPrefab(
				Property.serializedObject, 
				Property.FindPropertyRelative("m_List"),
				Property.displayName
			);

		#endregion


	}

}