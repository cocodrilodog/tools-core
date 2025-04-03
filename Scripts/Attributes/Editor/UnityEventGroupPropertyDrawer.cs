namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using UnityEditor;
	using UnityEditorInternal;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UIElements;

	[CustomPropertyDrawer(typeof(UnityEventGroupAttribute))]
	public class UnityEventGroupPropertyDrawer : UnityEventDrawer {


		#region Unity Methods

		// This is overriden to force OnGUI()
		public override VisualElement CreatePropertyGUI(SerializedProperty property) => null;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

			RegisterProperty(property.Copy(), (attribute as UnityEventGroupAttribute).GroupName, out var group, out var indexInGroup);

			if (indexInGroup == -1) {
				return (EditorGUIUtility.singleLineHeight + 2) * 2;
			} else if (indexInGroup == group.SelectedIndex) {
				var height = base.GetPropertyHeight(property, label); // Event height
				if (group.EventPaths.Count > 1) { // Add the toolbar height only when there is more than one event in the group
					height += EditorGUIUtility.singleLineHeight + 2; // The toolbar height
				}
				return height + group.EventPaths.Count * 1.5f; // Small compensation for the short height they use when not drawn
			} else {
				return 0;
			}

		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			label = EditorGUI.BeginProperty(position, label, property);

			RegisterProperty(property.Copy(), (attribute as UnityEventGroupAttribute).GroupName, out var group, out var indexInGroup);
			position.y -= indexInGroup * 1.5f; // Small compensation for the short height they use when not drawn

			if (indexInGroup == -1) {
				EditorGUI.HelpBox(position, $"{property.name}: UnityEventGroup attribute only supports UnityEvent classes.", MessageType.Error);	
			} else if (indexInGroup == group.SelectedIndex) {

				// Toolbar buttons
				var contents = new List<GUIContent>();
				foreach (var path in group.EventPaths) {
					var groupEventProperty = property.serializedObject.FindProperty(path);
					var size = groupEventProperty.FindPropertyRelative("m_PersistentCalls.m_Calls").arraySize;
					var text = size > 0 ? $"{groupEventProperty.displayName} ({size})" : groupEventProperty.displayName;
					contents.Add(new GUIContent(text, groupEventProperty.tooltip));
				}

				// Draw the tool bar only when there is more than one event in the group
				if (group.EventPaths.Count > 1) {

					var toolBarRect = position;
					toolBarRect.height = EditorGUIUtility.singleLineHeight;
					toolBarRect.xMin += EditorGUI.indentLevel * 15;

					group.SelectedIndex = GUI.Toolbar(toolBarRect, group.SelectedIndex, contents.ToArray());

				}

				// Selected event
				var eventRect = position;
				if (group.EventPaths.Count > 1) { // Move the event down only when there is a toolbar
					eventRect.y += EditorGUIUtility.singleLineHeight + 2;
				}

				base.OnGUI(eventRect, property, label);

			}

			EditorGUI.EndProperty();

		}

		#endregion


		#region Private Static Fields

		/// <summary>
		/// Because this property drawer will need data from other properties of the same object, this static
		/// map will hold a reference of each of the event properties, their corresponding groups and the 
		/// UnityEngine.Object that they belong to.
		/// </summary>
		private static Dictionary<UnityEngine.Object, Dictionary<string, List<Group>>> s_GroupsMap = 
			new Dictionary<UnityEngine.Object, Dictionary<string, List<Group>>>();

		#endregion


		#region Private Static Methods

		[InitializeOnLoadMethod]
		private static void InitOnLoad() {

			s_GroupsMap.Clear();

			Selection.selectionChanged -= Selection_selectionChanged;
			Selection.selectionChanged += Selection_selectionChanged;

		}

		/// <summary>
		/// This will keep the size of the map small
		/// </summary>
		private static void Selection_selectionChanged() {
			s_GroupsMap.Clear();
		}

		private static void RegisterProperty(SerializedProperty property, string groupName, out Group group, out int index) {

			// The target object may have multiple event owners (System.Object with events, for example)
			s_GroupsMap.TryAdd(property.serializedObject.targetObject, new Dictionary<string, List<Group>>());

			// The owners of the events held by the serialized object
			var owners = s_GroupsMap[property.serializedObject.targetObject];
			var nameIndex = property.propertyPath.IndexOf(property.name);
			var ownerPath = property.propertyPath.Substring(0, nameIndex);

			// ownerPath can be something like "m_SomeObject.", so we remove the last . for cleanliness
			if (ownerPath.Length > 0 && ownerPath[ownerPath.Length - 1] == '.') {
				ownerPath = ownerPath.Remove(ownerPath.Length - 1);
			}
			// This make it more clear
			if (ownerPath == "") {
				ownerPath = "root";
			}
			// Store the owner by its property path, up until before the event name
			owners.TryAdd(ownerPath, new List<Group>());

			// The owners can have multiple event groups
			var groups = owners[ownerPath];
			group = groups.FirstOrDefault(g => groupName == g.Name);
			if (group == null) {
				group = new Group(groupName);
				groups.Add(group);
			}
			index = group.AddEventPath(property.serializedObject, property.propertyPath);

		}

		#endregion


		public class Group {


			#region Public Properties

			public string Name => m_Name;

			public ReadOnlyCollection<string> EventPaths => m_EventPathsRO = m_EventPathsRO ?? new ReadOnlyCollection<string>(m_EventPaths);

			public int SelectedIndex {
				get => m_SelectedIndex;
				set => m_SelectedIndex = value;
			}

			#endregion


			#region Constructor

			public Group(string name) => m_Name = name;

			#endregion


			#region Public Methods

			public int AddEventPath(SerializedObject serializedObject, string eventPropertyPath) {
				if (!m_EventPaths.Contains(eventPropertyPath)) {

					var propertyType = CDEditorUtility.GetPropertyType(serializedObject.FindProperty(eventPropertyPath));
					var isUnityEvent = typeof(UnityEvent).IsAssignableFrom(propertyType) ||
						SystemUtility.IsSubclassOfRawGeneric(propertyType, typeof(UnityEvent<>)) ||
						SystemUtility.IsSubclassOfRawGeneric(propertyType, typeof(UnityEvent<,>)) ||
						SystemUtility.IsSubclassOfRawGeneric(propertyType, typeof(UnityEvent<,,>)) ||
						SystemUtility.IsSubclassOfRawGeneric(propertyType, typeof(UnityEvent<,,,>));

					if (isUnityEvent) {
						m_EventPaths.Add(eventPropertyPath);
					} else {
						return -1;
					}

				}
				return m_EventPaths.IndexOf(eventPropertyPath);
			}

			#endregion


			#region Private Fields

			private string m_Name;

			private int m_SelectedIndex;

			private List<string> m_EventPaths = new List<string>();

			private ReadOnlyCollection<string> m_EventPathsRO;

			#endregion


		}

	}

}
