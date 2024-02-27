namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Base class for editors of concrete implementations of <see cref="CompositeRoot"/>.
	/// </summary>
	[CustomEditor(typeof(CompositeRoot), true)]
	public class CompositeRootEditor : UnityEditor.Editor {


		#region Public Static Methods

		public static void SelectCompositeObject(
			SerializedObject serializedObject,
			string newPropertyPath
			) {

			var selectedCompositePath = ((CompositeRoot)serializedObject.targetObject).SelectedCompositePath;

			// Set to non-edit the previosly selected composite object
			if (!string.IsNullOrEmpty(selectedCompositePath)) {
				var selectedCompositeObjectProperty = serializedObject.FindProperty(selectedCompositePath);
				(selectedCompositeObjectProperty.managedReferenceValue as CompositeObject).Edit = false;
			}

			// Assign the new value
			((CompositeRoot)serializedObject.targetObject).SelectedCompositePath = newPropertyPath;

			// Set to edit the new selected composite object
			if (!string.IsNullOrEmpty(newPropertyPath)) {
				var selectedCompositeObjectProperty = serializedObject.FindProperty(newPropertyPath);
				(selectedCompositeObjectProperty.managedReferenceValue as CompositeObject).Edit = true;
			}

			// Set any selected field to be non-selected. Otherwise the text content of a selected field
			// will remain displayed, even if a new selected text field has other text.
			GUI.FocusControl(null);

		}

		#endregion


		#region Unity Methods

		protected virtual void OnEnable() {
			ScriptProperty = serializedObject.FindProperty("m_Script");
			m_SiblingsControlTexture = Resources.Load("CompositeSiblingsControl") as Texture;
		}

		public sealed override void OnInspectorGUI() {
			if (string.IsNullOrEmpty(SelectedCompositePath)) {
				// There is no selected composite object, proceed with the inspector of the root object
				OnRootInspectorGUI();
			} else {
				// There is a selected composite object, draw only it as a property.
				serializedObject.Update();
				CDEditorUtility.DrawDisabledField(ScriptProperty);
				var selectedCompositeProperty = serializedObject.FindProperty(SelectedCompositePath);
				if (selectedCompositeProperty != null) {
					DrawBreadcrums(selectedCompositeProperty);
					EditorGUILayout.PropertyField(selectedCompositeProperty);
				}
				serializedObject.ApplyModifiedProperties();
			}
			// This ensures the selected CompositeObject is editable.
			CheckEdit();
		}

		#endregion


		#region Protected Properties

		protected SerializedProperty ScriptProperty { get; private set; }

		#endregion


		#region Protected Methods

		/// <summary>
		/// In subclasses, override this instead of <see cref="OnInspectorGUI"/>.
		/// </summary>
		/// <remarks>
		/// This editor will choose to display this GUI when no <see cref="CompositeObject"/> is selected.
		/// Otherwise it will render the property drawer of the selected <see cref="CompositeObject"/>.
		/// </remarks>
		protected virtual void OnRootInspectorGUI() => base.OnInspectorGUI();

		#endregion


		#region Private Static Methods

		/// <summary>
		/// Checks whether the property corresponds to a CompositeObject and if so, assigns it to 
		/// <paramref name="compositeObject"/>
		/// </summary>
		/// <param name="property">The property</param>
		/// <param name="compositeObject">The CompositeObject to assign to</param>
		/// <returns><c>true</c> if the property corresponds to a CompositeObject, otherwise <c>false</c></returns>
		static private bool IsCompositeProperty(SerializedProperty property, out CompositeObject compositeObject) {
			var isComposite = property.propertyType == SerializedPropertyType.ManagedReference &&
				property.managedReferenceValue is CompositeObject;
			compositeObject = null;
			if (isComposite) {
				compositeObject = property.managedReferenceValue as CompositeObject;
			}
			return isComposite;
		}

		#endregion


		#region Private Fields

		private Texture m_SiblingsControlTexture;

		#endregion


		#region Private Properties

		private string SelectedCompositePath => ((CompositeRoot)target).SelectedCompositePath;

		#endregion


		#region Private Methods

		private void DrawBreadcrums(SerializedProperty property) {

			GUILayout.BeginHorizontal();

			if (!string.IsNullOrEmpty(SelectedCompositePath)) {

				// This button will make the inspector to go back to the root
				DrawNextButton($"◂ ", $"{target.GetType().Name}", () => {
					SelectCompositeObject(serializedObject, null);
				});

				// Analize path parts and create a breadcrum button for each CompositeObject in the path
				var pathParts = property.propertyPath.Split('.');
				SerializedProperty parentProperty = null;

				for (int i = 0; i < pathParts.Length; i++) {

					// Each path until the i part
					var partialPath = string.Join('.', pathParts, 0, i + 1);
					var partialProperty = property.serializedObject.FindProperty(partialPath);

					if (partialProperty != null) {
						if (IsCompositeProperty(partialProperty, out var partialComposite)) {

							// Composite for this partial path
							if (partialComposite == property.managedReferenceValue) {
								// The partialComposite is the main composite object of this property
								DrawNextButton($"• ", partialProperty, parentProperty);
							} else {
								// The partialComposite is an intermediate between the root and the main
								// composite object of this property
								DrawNextButton($"◂ ", partialProperty, parentProperty, () => {
									SelectCompositeObject(serializedObject, partialProperty.propertyPath);
								});
							}

							parentProperty = partialProperty.Copy();

						}
					}

				}

			}
			GUILayout.EndHorizontal();

		}

		/// <summary>
		/// Draws a breadcrums button.
		/// </summary>
		/// <param name="label">The label of the button</param>
		/// <param name="action">The action that the button will perform</param>
		private void DrawNextButton(string prefix, string label, Action action = null) {
			EditorGUI.BeginDisabledGroup(action == null);
			if (GUILayout.Button(prefix + label, GUILayout.ExpandWidth(false))) {
				action?.Invoke();
			}
			// Reduce the space between the buttons
			//GUILayout.Space(-3);
			EditorGUI.EndDisabledGroup();
		}

		/// <summary>
		/// Draws a breadcrums button that can show a <see cref="GenericMenu"/> with the siblings 
		/// of the <see cref="CompositeObject"/> of the button.
		/// </summary>
		/// <param name="prefix">A prefix for the button. For example "•" or "◂".</param>
		/// <param name="currentProperty">The property that corresponds to the button</param>
		/// <param name="parentProperty">
		/// The parent property to iterate over its children and display them in a <see cref="GenericMenu"/>.
		/// They are the siblings of the button's object.
		/// </param>
		/// <param name="action">The action that the button will perform</param>
		private void DrawNextButton(string prefix, SerializedProperty currentProperty, SerializedProperty parentProperty, Action action = null) {

			// Create a siblings menu
			var siblingsMenu = new SiblingsMenu(serializedObject, currentProperty, parentProperty);

			// Draw the button
			EditorGUI.BeginDisabledGroup(action == null);

			// Get the the button label from the menu, because it may be a repeated DisplayName that
			// need to be changed to DisplayName (#)
			if (GUILayout.Button(prefix + siblingsMenu.ButtonLabel, GUILayout.ExpandWidth(false))) {
				action?.Invoke();
			}

			// Get the siblings rect right after drawing the button
			var siblingsRect = GUILayoutUtility.GetLastRect();
			siblingsRect.x = siblingsRect.xMax;
			siblingsRect.width = 14;
			var center = siblingsRect.center;

			// Reduce the space between the buttons
			GUILayout.Space(14);
			EditorGUI.EndDisabledGroup();

			if (GUI.Button(siblingsRect, "")) {
				siblingsMenu.ShowAsContext();
			}

			// Create the up and down icon
			var iconRect = siblingsRect;
			iconRect.height = 14;
			iconRect.center = center;
			GUI.DrawTexture(iconRect, m_SiblingsControlTexture);

		}

		private void CheckEdit() {
			if (!string.IsNullOrEmpty(SelectedCompositePath)) {
				var selectedCompositeObjectProperty = serializedObject.FindProperty(SelectedCompositePath);
				(selectedCompositeObjectProperty.managedReferenceValue as CompositeObject).Edit = true;
			}
		}

		#endregion


		public class SiblingsMenu {


			#region public Properties

			public string ButtonLabel => m_ButtonLabel;

			#endregion


			#region Public Constructors

			public SiblingsMenu(SerializedObject serializedObject, SerializedProperty currentProperty, SerializedProperty parentProperty) {

				m_Menu = new GenericMenu();

				m_SerializedObject = serializedObject;
				m_CurrentProperty = currentProperty;
				m_ParentProperty = parentProperty;

				if (parentProperty != null) {
					// Parent property is CompositeObject
					CDEditorUtility.IterateChildProperties(m_ParentProperty, ForEachSibling);
				} else {
					// Parent property is CompositeRoot
					CDEditorUtility.IterateChildProperties(m_SerializedObject, ForEachSibling);
				}

			}

			#endregion


			#region Public Methods

			public void ShowAsContext() {
				GUI.FocusControl(null);
				m_Menu.ShowAsContext();
			}

			#endregion


			#region Private Fields

			private SerializedObject m_SerializedObject;

			private SerializedProperty m_CurrentProperty;

			private SerializedProperty m_ParentProperty;

			private Dictionary<string, int> m_SiblingNamesCount = new Dictionary<string, int>();

			private string m_ButtonLabel;

			private GenericMenu m_Menu;

			#endregion


			#region Private Methods

			private void ForEachSibling(SerializedProperty siblingProperty) {
				if (IsCompositeProperty(siblingProperty, out var compositeSibling)) {

					// Save for later use by the menu
					var pendingSiblingProperty = siblingProperty.Copy();
					var on = siblingProperty.propertyPath == m_CurrentProperty.propertyPath;
					var checkedName = CheckRepeatedName(compositeSibling.DisplayName);

					m_Menu.AddItem(new GUIContent(checkedName), on, () => SelectSibling(pendingSiblingProperty));

					if (siblingProperty.propertyPath == m_CurrentProperty.propertyPath) {
						m_ButtonLabel = checkedName;
					}

				} else if (siblingProperty.isArray && siblingProperty.propertyType == SerializedPropertyType.Generic) {
					// The property is an array or list
					for (int i = 0; i < siblingProperty.arraySize; i++) {
						
						// Get the elements
						var element = siblingProperty.GetArrayElementAtIndex(i);
						
						if (element.propertyType == SerializedPropertyType.ManagedReference) {
							
							// It is managed reference
							compositeSibling = element.managedReferenceValue as CompositeObject;
							
							if (compositeSibling != null) {

								// It is CompositeObject, save for later use by the menu
								var pendingElement = element.Copy();
								var on = element.propertyPath == m_CurrentProperty.propertyPath;
								var checkedName = CheckRepeatedName(compositeSibling.DisplayName);

								m_Menu.AddItem(new GUIContent(checkedName), on, () => SelectSibling(pendingElement));

								if (element.propertyPath == m_CurrentProperty.propertyPath) {
									m_ButtonLabel = checkedName;
								}

							} else {
								// These are managed references, but not CompositeObjects 
								break;
							}

						} else {
							// This is an array of non-managed reference objects
							break;
						}
					
					} 
				} else if (SystemUtility.IsSubclassOfRawGeneric(CDEditorUtility.GetPropertyType(siblingProperty), typeof(CompositeList<>))) {
					// The property is a CompositeList<>
					var internalList = siblingProperty.FindPropertyRelative("m_List");
					for(int i = 0; i < internalList.arraySize; i++) {

						var element = internalList.GetArrayElementAtIndex(i);
						compositeSibling = element.managedReferenceValue as CompositeObject;

						// Elements could be null
						if (compositeSibling != null) {

							// Save for later use by the menu
							var pendingSiblingProperty = element.Copy();
							var on = element.propertyPath == m_CurrentProperty.propertyPath;
							var checkedName = CheckRepeatedName(compositeSibling.DisplayName);

							m_Menu.AddItem(new GUIContent(checkedName), on, () => SelectSibling(pendingSiblingProperty));

							if (element.propertyPath == m_CurrentProperty.propertyPath) {
								m_ButtonLabel = checkedName;
							}

						}

					}
				}
			}

			/// <summary>
			/// Checks if the name is repeated and if so, appends a counter like "(1)", "(2)", etc.
			/// </summary>
			/// <param name="siblingName"></param>
			/// <param name="names"></param>
			/// <returns></returns>
			private string CheckRepeatedName(string siblingName) {
				if (!m_SiblingNamesCount.ContainsKey(siblingName)) {
					m_SiblingNamesCount[siblingName] = 1;
				} else {
					m_SiblingNamesCount[siblingName]++;
					siblingName += $" ({m_SiblingNamesCount[siblingName] - 1})";
				}
				return siblingName;
			}

			/// <summary>
			/// Delay the selection to overcome strange menu timing.
			/// </summary>
			/// <param name="prop">The property of the composite object to be selected</param>
			private void SelectSibling(SerializedProperty prop) {
				EditorApplication.delayCall += () => {
					m_SerializedObject.Update();
					SelectCompositeObject(m_SerializedObject, prop.propertyPath);
					m_SerializedObject.ApplyModifiedProperties();
				};
			}

			#endregion


		}

	}

}