namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;


	/// <summary>
	/// Base class for editors of concrete implementations of <see cref="ICompositeRoot"/>.
	/// </summary>
	public class CompositeRootEditor : Editor {


		#region Public Static Methods

		/// <summary>
		/// Selects a composite, and makes the inspector to show the property drawer of that object as well
		/// as updating the breadcrumb buttons, and sibling menus.
		/// </summary>
		/// <param name="serializedObject">The <see cref="SerializedObject"/> that corresponds to this <see cref="ICompositeRoot"/></param>
		/// <param name="newPropertyPath">The property path of the <see cref="CompositeObject"/> that will be selected</param>
		public static void SelectCompositeObject(SerializedObject serializedObject, string newPropertyPath) {

			var selectedCompositePath = ((ICompositeRoot)serializedObject.targetObject).SelectedCompositePath;

			// Set to non-edit the previosly selected composite object
			if (!string.IsNullOrEmpty(selectedCompositePath)) {
				var selectedCompositeObjectProperty = serializedObject.FindProperty(selectedCompositePath);
				(selectedCompositeObjectProperty.managedReferenceValue as CompositeObject).Edit = false;
			}

			// Assign the new value
			((ICompositeRoot)serializedObject.targetObject).SelectedCompositePath = newPropertyPath;

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
					DrawBreadcrumbs(selectedCompositeProperty);
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

		private string SelectedCompositePath => ((ICompositeRoot)target).SelectedCompositePath;

		#endregion


		#region Private Methods

		private void DrawBreadcrumbs(SerializedProperty property) {

			// Collect the breadcrumbs data before drawing the buttons
			var breadcrumbRows = new List<List<Breadcrumb>>();
			breadcrumbRows.Add(new List<Breadcrumb>());

			// These numbers don't match the image analyzed in Photoshop, but they work better.
			var x = 12f;
			var layoutSpace = 10;
			var siblingsButtonWidth = 12;

			if (!string.IsNullOrEmpty(SelectedCompositePath)) {

				// This corresponds to the first button that will make the inspector to go back to the root
				var breadcrumb = new Breadcrumb();
				breadcrumb.Prefix = $"";
				breadcrumb.Label = target.GetType().Name;
				breadcrumb.Action = () => SelectCompositeObject(serializedObject, null);
				breadcrumbRows[breadcrumbRows.Count - 1].Add(breadcrumb);

				// Add the size to the total width
				var buttonSize = GUI.skin.button.CalcSize(new GUIContent(breadcrumb.Prefix + breadcrumb.Label));
				buttonSize.x = Mathf.Ceil(buttonSize.x);
				x += layoutSpace + buttonSize.x;

				// Analize path parts and create a breadcrumb button for each CompositeObject in the path
				var pathParts = property.propertyPath.Split('.');
				SerializedProperty parentProperty = null;

				for (int i = 0; i < pathParts.Length; i++) {

					// Each path until the i part
					var partialPath = string.Join('.', pathParts, 0, i + 1);
					var partialProperty = property.serializedObject.FindProperty(partialPath);

					if (partialProperty != null) {
						if (IsCompositeProperty(partialProperty, out var partialComposite)) {

							// This was taken out so that we can get the label of the button beforehand
							var siblingsMenu = new SiblingsMenu(serializedObject, partialProperty, parentProperty);

							// These are the buttons starting from the second one so they correspond to composite objects
							breadcrumb = new Breadcrumb();
							breadcrumb.CurrentProperty = partialProperty;
							breadcrumb.ParentProperty = parentProperty;
							breadcrumb.SiblingsMenu = siblingsMenu;

							// Add the size to the total width
							buttonSize = GUI.skin.button.CalcSize(new GUIContent(breadcrumb.Prefix + breadcrumb.SiblingsMenu.ButtonLabel));
							buttonSize.x = Mathf.Ceil(buttonSize.x);
							x += layoutSpace + buttonSize.x + siblingsButtonWidth;
							
							// If this button will pass the right limit of the inspector, create a new row of breadcrums
							// and reset x
							if (x > EditorGUIUtility.currentViewWidth) {
								x = 12;
								breadcrumbRows.Add(new List<Breadcrumb>());
							}

							// Set the icon and prefix
							breadcrumb.Icon = CompositeObjectPropertyDrawer.GetObjectIcon(partialComposite.GetType());
							if (breadcrumb.Icon != null) {
								breadcrumb.Prefix = $"    ";
							} else {
								breadcrumb.Prefix = $"";
							}

							// Composite for this partial path
							if (partialComposite == property.managedReferenceValue) {
								// The partialComposite is the main composite object of this property
								// No action to assign.
							} else {
								// The partialComposite is an intermediate between the root and the main
								breadcrumb.Action = () => SelectCompositeObject(serializedObject, partialProperty.propertyPath);
							}

							// Add the breadcrumb to the last added row
							breadcrumbRows[breadcrumbRows.Count - 1].Add(breadcrumb);
							parentProperty = partialProperty.Copy();

						}

					}

				}

				// Draw the breadcrumb buttons
				for(int i = 0; i < breadcrumbRows.Count; i++) {

					GUILayout.BeginHorizontal();

					for(int j = 0; j < breadcrumbRows[i].Count; j++) {
						var b = breadcrumbRows[i][j];
						if (i == 0 && j == 0) {
							// The first button uses this overload
							DrawNextButton(b.Prefix, b.Label, b.Action);
						} else {
							// Starting with the second button, the buttons use this overload
							DrawNextButton(b.Icon, b.Prefix, b.SiblingsMenu, !(i == breadcrumbRows.Count - 1 && j == breadcrumbRows[i].Count - 1), b.Action);
						}
					}

					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();

				}

			}

		}

		/// <summary>
		/// Draws a breadcrums button.
		/// </summary>
		/// <param name="label">The label of the button</param>
		/// <param name="action">The action that the button will perform</param>
		private void DrawNextButton(string prefix, string label, Action action = null) {
			EditorGUI.BeginDisabledGroup(action == null);

			GUILayout.Space(-3);
			if (GUILayout.Button(prefix + label, GUILayout.ExpandWidth(false))) {
				action?.Invoke();
			}
			// Reduce the space between the buttons
			//GUILayout.Space(-3);
			EditorGUI.EndDisabledGroup();

			GUILayout.Space(-3);
			GUILayout.Label("▸");
		}

		/// <summary>
		/// Draws a breadcrums button that can show a <see cref="GenericMenu"/> with the siblings 
		/// of the <see cref="CompositeObject"/> of the button.
		/// </summary>
		/// <param name="icon">An optional icon to represent the object.</param>
		/// <param name="prefix">A prefix for the button. For example "•" or "◂".</param>
		/// <param name="currentProperty">The property that corresponds to the button</param>
		/// <param name="parentProperty">
		/// The parent property to iterate over its children and display them in a <see cref="GenericMenu"/>.
		/// They are the siblings of the button's object.
		/// </param>
		/// <param name="action">The action that the button will perform</param>
		private void DrawNextButton(Texture icon, string prefix, SiblingsMenu siblingsMenu, bool rightTriangle, Action action = null) {

			GUILayout.BeginHorizontal();

			// Draw the main button
			EditorGUI.BeginDisabledGroup(action == null);

			GUILayout.Space(-3);
			// Get the the button label from the menu, because it may be a repeated DisplayName that
			// need to be changed to DisplayName (#)
			if (GUILayout.Button(prefix + siblingsMenu.ButtonLabel, GUILayout.ExpandWidth(false))) {
				action?.Invoke();
			}

			if(icon != null) {
				var objectIconRect = GUILayoutUtility.GetLastRect();
				objectIconRect.x += 2;
				objectIconRect.y += 2;
				objectIconRect.size = new Vector2(16, 16);
				GUI.DrawTexture(objectIconRect, icon);
			}

			// Reduce the space between the buttons
			GUILayout.Space(-3);
			EditorGUI.EndDisabledGroup();

			// Draw sibling button
			if (GUILayout.Button("", GUILayout.ExpandWidth(false))) {
				siblingsMenu.ShowAsContext();
			}

			// Create the up and down icon
			var siblingsIconRect = GUILayoutUtility.GetLastRect();
			GUI.DrawTexture(siblingsIconRect, m_SiblingsControlTexture);

			if (rightTriangle) {
				GUILayout.Space(-3);
				GUILayout.Label("▸");
			}

			GUILayout.EndHorizontal();

		}

		private void CheckEdit() {
			if (!string.IsNullOrEmpty(SelectedCompositePath)) {
				var selectedCompositeObjectProperty = serializedObject.FindProperty(SelectedCompositePath);
				var selectedCompositeObject = (selectedCompositeObjectProperty.managedReferenceValue as CompositeObject);
				if (selectedCompositeObject != null) {
					selectedCompositeObject.Edit = true;
				}
			}
		}

		#endregion

		/// <summary>
		/// Creates the context menu that will show the siblings of the current composite object, and show it.
		/// </summary>
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


		/// <summary>
		/// Collects data for each breadcrumb button.
		/// </summary>
		public class Breadcrumb {


			#region Public Fields

			public Texture Icon;

			public string Prefix;
			
			public string Label;
			
			public SerializedProperty CurrentProperty;
			
			public SerializedProperty ParentProperty;
			
			public SiblingsMenu SiblingsMenu;
			
			public Action Action;

			#endregion


		}

	}

}