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
					
					EditorGUI.BeginChangeCheck();
					group.SelectedIndex = GUI.Toolbar(toolBarRect, group.SelectedIndex, contents.ToArray());
					if (EditorGUI.EndChangeCheck()) {
						GUI.FocusControl(null);
						Debug.Log("CD: OnGUI() Changed group");
					}

				}

				// Selected event
				var eventRect = position;
				if (group.EventPaths.Count > 1) { // Move the event down only when there is a toolbar
					eventRect.y += EditorGUIUtility.singleLineHeight + 2;
				}

				base.OnGUI(eventRect, property, label);

			}

			EditorGUI.EndProperty();

			if (property.contentHash != m_previousPropertyContentHash) {
				s_EventChanged = true;
				Debug.Log("CD: OnGUI() Event Changed");
			} 
			m_previousPropertyContentHash = property.contentHash;

		}

		private uint m_previousPropertyContentHash;

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
		private static Dictionary<UnityEngine.Object, Dictionary<string, List<Group>>> s_GroupsMap = 
			new Dictionary<UnityEngine.Object, Dictionary<string, List<Group>>>();

		/// <summary>
		/// Since the <see cref="s_GroupsMap"/> is being reset when any property of any object changes, this flag prevents
		/// it from being reset when the change was caused by the event property, which would othewise make it harder to use.
		/// </summary>
		private static bool s_EventChanged;

		#endregion


		#region Private Static Methods

		[InitializeOnLoadMethod]
		private static void InitOnLoad() {

			s_EventChanged = false;
			s_GroupsMap.Clear();
			Debug.Log("CD: InitOnLoad() s_GroupsMap.Clear()");

			Selection.selectionChanged -= Selection_selectionChanged;
			Selection.selectionChanged += Selection_selectionChanged;

			//ObjectChangeEvents.changesPublished -= ObjectChangeEvents_changesPublished;
			//ObjectChangeEvents.changesPublished += ObjectChangeEvents_changesPublished;

		}

		/// <summary>
		/// This will keep the size of the map small
		/// </summary>
		private static void Selection_selectionChanged() {
			s_EventChanged = false;
			s_GroupsMap.Clear();
			Debug.Log("CD: Selection_selectionChanged() s_GroupsMap.Clear()");
		}

		/// <summary>
		/// This prevents errors when an array on the serializedObject changes.
		/// </summary>
		/// <param name="stream"></param>
		//private static void ObjectChangeEvents_changesPublished(ref ObjectChangeEventStream stream) {
		//	for (int i = 0; i < stream.length; ++i) {
		//		switch (stream.GetEventType(i)) {
		//			case ObjectChangeKind.ChangeGameObjectOrComponentProperties:
		//				// Here we give time to the s_EventChanged before reading it because sometimes
		//				// this method is invoked before the m_EventChanged flag is set to true.
		//				Action action = () => {
		//					if (s_EventChanged) {
		//						s_EventChanged = false;
		//					} else {
		//						s_GroupsMap.Clear();
		//						Debug.Log("CD: ObjectChangeEvents_changesPublished(...) s_GroupsMap.Clear()");
		//					}
		//				};
		//				CDEditorUtility.DelayedAction(action, 0.5f, "UnityEventGroup");
		//				break;
		//		}
		//	}
		//}


		private static void RegisterProperty(SerializedProperty property, string groupName, out Group group, out int index) {

			// The target object may have multiple event owners (System.Object with events, for example)
			if (!s_GroupsMap.ContainsKey(property.serializedObject.targetObject)) {
				s_GroupsMap[property.serializedObject.targetObject] = new Dictionary<string, List<Group>>();
				Debug.Log("CD: RegisterProperty(...) Created onwners dictionary");
			}

			// The owners of the events held by the serialized object
			var owners = s_GroupsMap[property.serializedObject.targetObject];
			var nameIndex = property.propertyPath.IndexOf(property.name);
			var ownerPath = property.propertyPath.Substring(0, nameIndex);

			// ownerPath can be something like "m_SomeMonoBehaviour.m_SomeObject.", so we remove the last . for cleanliness
			if (ownerPath.Length > 0 && ownerPath[ownerPath.Length - 1] == '.') {
				ownerPath = ownerPath.Remove(ownerPath.Length - 1);
			}
			// This make it more clear
			if (ownerPath == "") {
				ownerPath = "root";
			}
			if (!owners.ContainsKey(ownerPath)) { // Store the owner by its property path, up until the event name
				owners[ownerPath] = new List<Group>();
				Debug.Log($"\tCD: RegisterProperty(...) Created onwner: {ownerPath}");
			}

			// The owners can have multiple event groups
			var groups = owners[ownerPath];
			group = groups.FirstOrDefault(g => groupName == g.Name);
			if (group == null) {
				group = new Group(groupName);
				Debug.Log($"\t\tCD: RegisterProperty(...) Created group: {groupName}");
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
						SystemUtility.IsSubclassOfRawGeneric(propertyType, typeof(UnityEvent<>));

					if (isUnityEvent) {
						Debug.Log($"\t\t\tCD: AddEventPath(...) Added event path [{m_EventPaths.Count}]: {eventPropertyPath}");
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

		//public class Entry {


		//	#region Public Properties

		//	public string PropertyPath => m_PropertyPath;

		//	#endregion


		//	#region Constructor

		//	public Entry(string propertyPath) {
		//		m_PropertyPath = propertyPath;
		//	}

		//	#endregion


		//	#region Private Fields

		//	private string m_PropertyPath;

		//	#endregion


		//}

	}

}
