namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using UnityEditor;
	using UnityEditorInternal;
	using UnityEngine;
	using UnityEngine.UIElements;

	[CustomPropertyDrawer(typeof(UnityEventGroupAttribute))]
	public class UnityEventGroupPropertyDrawer : UnityEventDrawer {


		#region Unity Methods

		// This is overriden to force OnGUI()
		public override VisualElement CreatePropertyGUI(SerializedProperty property) => null;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

			RegisterProperty(property.Copy(), (attribute as UnityEventGroupAttribute).GroupName, out var group, out var indexInGroup);

			if (indexInGroup == group.SelectedIndex) {
				var height = base.GetPropertyHeight(property, label);
				height += EditorGUIUtility.singleLineHeight + 2;
				return height + group.Entries.Count * 1.5f;
			} else {
				return 0;
			}

		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			label = EditorGUI.BeginProperty(position, label, property);

			RegisterProperty(property.Copy(), (attribute as UnityEventGroupAttribute).GroupName, out var group, out var indexInGroup);
			position.y -= indexInGroup * 1.5f;

			if (indexInGroup == group.SelectedIndex) {

				var contents = new List<GUIContent>();
				foreach (var entry in group.Entries) {
					var size = entry.Property.FindPropertyRelative("m_PersistentCalls.m_Calls").arraySize;
					var text = size > 0 ? $"{entry.Property.displayName} ({size})" : entry.Property.displayName;
					contents.Add(new GUIContent(text, entry.Property.tooltip));
				}

				var toolBarRect = position;
				toolBarRect.height = EditorGUIUtility.singleLineHeight;
				group.SelectedIndex = GUI.Toolbar(toolBarRect, group.SelectedIndex, contents.ToArray());

				var eventRect = position;
				eventRect.y += EditorGUIUtility.singleLineHeight + 2;
				base.OnGUI(eventRect, property, label);

			}

			EditorGUI.EndProperty();

		}

		#endregion


		#region Private Static Fields

		/// <summary>
		/// Because this property drawer will need data from other properties of the same object, this static
		/// map will hold a reference of each of the event properties, their corresponding groups and the 
		/// SerializedObject that they belong to.
		/// </summary>
		/// 
		/// <remarks>
		/// Using the serializedObject as the key helps to store only the valid serialized properties. Sometimes, 
		/// the serialized object of a target object is renewed, making the old one and its properties invalid.
		/// Therefore, is we store the lates serialized object with its properties, we know that it and its properties
		/// are valid.
		/// </remarks>
		private static Dictionary<SerializedObject, List<Group>> s_GroupsMap = new Dictionary<SerializedObject, List<Group>>();

		#endregion


		#region Private Static Methods

		[InitializeOnLoadMethod]
		private static void InitOnLoad() {
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
			if (!s_GroupsMap.ContainsKey(property.serializedObject)) {
				s_GroupsMap[property.serializedObject] = new List<Group>();
			}
			var groups = s_GroupsMap[property.serializedObject];
			group = groups.FirstOrDefault(g => groupName == g.Name);
			if (group == null) {
				group = new Group(groupName);
				groups.Add(group);
			}
			index = group.AddEntry(property);
		}

		#endregion


		public class Group {


			#region Public Properties

			public string Name => m_Name;

			public ReadOnlyCollection<Entry> Entries => m_EntriesRO = m_EntriesRO ?? new ReadOnlyCollection<Entry>(m_Entries);

			public int SelectedIndex {
				get => m_SelectedIndex;
				set => m_SelectedIndex = value;
			}

			#endregion


			#region Constructor

			public Group(string name) => m_Name = name;

			#endregion


			#region Public Methods

			public int AddEntry(SerializedProperty property) {
				var entry = m_Entries.FirstOrDefault(e => property.propertyPath == e.Property.propertyPath);
				if (entry == null) {
					entry = new Entry(property);
					m_Entries.Add(entry);
				}
				return m_Entries.IndexOf(entry);
			}

			#endregion


			#region Private Fields

			private string m_Name;

			private int m_SelectedIndex;

			private List<Entry> m_Entries = new List<Entry>();

			private ReadOnlyCollection<Entry> m_EntriesRO;

			#endregion


		}

		public class Entry {


			#region Public Properties

			public SerializedProperty Property => m_Property;

			#endregion


			#region Constructor

			public Entry(SerializedProperty property) {
				m_Property = property;
			}

			#endregion


			#region Private Fields

			private SerializedProperty m_Property;

			#endregion


		}

	}

}
