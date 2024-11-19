namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(DocumentationComment))]
	public class DocumentationCommentPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			base.GetPropertyHeight(property, label);
			if (Comment.EditDocumentationComment) {
				return GetBoxHeight() + EditorGUIUtility.singleLineHeight + 2;
			} else {
				return GetBoxHeight();
			}
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			
			base.OnGUI(position, property, label);

			// Unity bug, sometimes I receive a value of 1 here
			m_Width = Position.width > 1 ? Position.width : m_Width;

			var height = GetBoxHeight();
			var boxRect = GetNextPosition(height);
			var buttonRect = boxRect;
		
			if (Comment.EditDocumentationComment) {

				// Draw the text area
				var wordWrappedStyle = new GUIStyle(EditorStyles.textArea);
				wordWrappedStyle.wordWrap = true;
				m_CommentProperty.stringValue = EditorGUI.TextArea(boxRect, m_CommentProperty.stringValue, wordWrappedStyle);

				// The done button
				buttonRect.y += height + 2;
				buttonRect.height = EditorGUIUtility.singleLineHeight;

				var guiContent = new GUIContent("Done!");
				if (GUI.Button(buttonRect, guiContent)) {
					Comment.EditDocumentationComment = false;
				}

			} else {

				var buttonSize = 16;
				buttonRect.xMin = buttonRect.xMax - buttonSize;
				buttonRect.height = buttonSize;

				var buttonRectCenter = buttonRect.center;
				buttonRectCenter.y = boxRect.center.y;
				buttonRect.center = buttonRectCenter;

				var helpIcon = EditorGUIUtility.IconContent("_Help@2x");

				if (!string.IsNullOrEmpty(m_CommentProperty.stringValue)) {

					// Draw the help box
					boxRect.xMax -= buttonSize;
					EditorGUI.HelpBox(boxRect, m_CommentProperty.stringValue, MessageType.Info);

					// The small button with the checkmark
					helpIcon.tooltip = "Click to edit.";
					if (GUI.Button(buttonRect, helpIcon, EditorStyles.iconButton)) {
						Comment.EditDocumentationComment = true;
					}

				} else {
					// The small button with the checkmark
					helpIcon.tooltip = "Click to add a documentation comment.";
					if (GUI.Button(buttonRect, helpIcon, EditorStyles.iconButton)) {
						Comment.EditDocumentationComment = true;
					}
				}

			}

		}

		#endregion


		#region Protected Methods

		protected override void InitializePropertiesForGetHeight() {
			base.InitializePropertiesForGetHeight();
			m_CommentProperty = Property.FindPropertyRelative("m_Comment");
		}

		protected override void InitializePropertiesForOnGUI() {
			base.InitializePropertiesForOnGUI();
			m_CommentProperty = Property.FindPropertyRelative("m_Comment");
		}

		#endregion


		#region Private Fields

		private SerializedProperty m_CommentProperty;

		private DocumentationComment m_Comment;

		private float m_Width;

		#endregion


		#region Private Properties

		private DocumentationComment Comment => 
			m_Comment = m_Comment ?? CDEditorUtility.GetPropertyValue(Property) as DocumentationComment;

		#endregion


		#region Private Methods

		public float GetBoxHeight() {
			if (Comment.EditDocumentationComment) {
				return EditorStyles.textArea.CalcHeight(
					new GUIContent(m_CommentProperty.stringValue),
					m_Width // Start with the width of the property
					- EditorGUI.indentLevel * 15 // Subtract the indent 
				);
			} else {
				return EditorStyles.helpBox.CalcHeight(
					new GUIContent(m_CommentProperty.stringValue),
					m_Width // Start with the width of the property
					- EditorGUI.indentLevel * 15 // Subtract the indent 
					- 32 // Subtract the help icon and space
					- 16 // Subtract right button space
				);
			}
		}

		#endregion


	}

}