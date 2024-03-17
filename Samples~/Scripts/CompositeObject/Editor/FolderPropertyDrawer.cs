namespace CocodriloDog.Core.Examples.Editor {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// This is no needed anymore since the creation of <c>CompositeList&lt;&gt;</c>. 
	/// </summary>
	/// 
	/// <remarks>
	/// This would be the way to create a custom property drawer for the <see cref="Folder"/> class if the
	/// <c>m_Files</c> property was of type List&lt;FileBase&gt;. It was intended to protect the objects from 
	/// data corruption when using prefabs, but that is now automatically handled by <c>CompositeList&lt;&gt;</c>.
	/// I am saving it just for reference.
	/// </remarks>
	//[CustomPropertyDrawer(typeof(Folder))]
	public class FolderPropertyDrawer : CompositePropertyDrawer {


		#region Protected Properties

		protected override bool UseDefaultDrawer => false;

		#endregion


		#region Protected Methods

		protected override float Edit_GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var height = base.Edit_GetPropertyHeight(property, label);
			var filesProperty = Property.FindPropertyRelative("m_Files");
			height += EditorGUI.GetPropertyHeight(filesProperty) + 2;
			return height;
		}

		protected override void Edit_OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			base.Edit_OnGUI(position, property, label);
			var filesProperty = Property.FindPropertyRelative("m_Files");
			FilesDrawer.DoList(GetNextPosition(filesProperty), filesProperty);
		}

		#endregion


		#region Private Fields

		private CompositeListPropertyDrawerForPrefab m_FilesDrawer;

		#endregion


		#region Private Properties

		private CompositeListPropertyDrawerForPrefab FilesDrawer => 
			m_FilesDrawer = m_FilesDrawer ?? new CompositeListPropertyDrawerForPrefab(
				Property.FindPropertyRelative("m_Files")
			);

		#endregion


	}

}