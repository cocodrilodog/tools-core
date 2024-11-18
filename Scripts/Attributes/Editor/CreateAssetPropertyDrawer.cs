namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(CreateAssetAttribute))]
	public class CreateAssetPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);

			label = EditorGUI.BeginProperty(Position, Label, Property);

			// Pending creation command
			if (m_CreateAssetCommand != null && m_CreateAssetCommand.Asset != null) {
				// Assign the created asset to the property
				Property.objectReferenceValue = m_CreateAssetCommand.Asset;
				m_CreateAssetCommand = null;
			}

			// Check if the property is null
			if (Property.objectReferenceValue == null) {

				// Draw a smaller property field 
				var width = 60;
				var fieldRect = Position;
				fieldRect.xMax -= width + 2;
				EditorGUI.PropertyField(fieldRect, Property, Label);

				// Create a button next to the property field
				var buttonRect = new Rect(Position.xMax - width, Position.y, width, Position.height);
				if (GUI.Button(buttonRect, "Create")) {

					// Get the type of the ScriptableObject
					var objectType = CDEditorUtility.GetPropertyType(Property);

					// Collect options
					List<Type> types = SystemUtility.GetConcreteDerivedTypes(objectType);

					// Show the menu only when there are more than one types
					if (types.Count > 1) {
						var menu = new GenericMenu();
						foreach (var type in types) {
							menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(type.Name)), false, () => {
								// Store data for the creation of the asset on next frame to prevent editor errors
								m_CreateAssetCommand = new CreateAssetCommand(type);
								EditorApplication.update += EditorApplication_update;
							});
						}
						menu.ShowAsContext();
					} else {
						// When there is only one type, create the object immediatly
						// Store data for the creation of the asset on next frame to prevent editor errors
						m_CreateAssetCommand = new CreateAssetCommand(types[0]);
						EditorApplication.update += EditorApplication_update;
					}

				}

			} else {
				EditorGUI.PropertyField(Position, Property, Label);
			}

			EditorGUI.EndProperty();

		}

		#endregion


		#region Event Handlers

		private void EditorApplication_update() {
			EditorApplication.update -= EditorApplication_update;
			m_CreateAssetCommand.Excecute();
		}

		#endregion


		#region Private Fields

		private CreateAssetCommand m_CreateAssetCommand;

		#endregion


		#region Support Classes

		private class CreateAssetCommand {

			public ScriptableObject Asset => m_Asset;

			public CreateAssetCommand(Type type) {
				m_Type = type;
			}

			public void Excecute() {

				// Create a new ScriptableObject instance
				m_Asset = ScriptableObject.CreateInstance(m_Type);

				// Save the asset to the root of the Assets folder
				string path = EditorUtility.SaveFilePanelInProject(
					"Create New ScriptableObject",
					"New" + m_Type.Name + ".asset",
					"asset",
					"Create a new ScriptableObject",
					"Assets"
				);

				if (!string.IsNullOrEmpty(path)) {
					AssetDatabase.CreateAsset(m_Asset, path);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}

			}
			
			private Type m_Type;

			private ScriptableObject m_Asset;

		}

		#endregion


	}

}