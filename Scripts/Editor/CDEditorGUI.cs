namespace CocodriloDog.Core {

	using UnityEditor;
	using UnityEngine;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public static class CDEditorGUI {


		#region Public Static Methods

		/// <summary>
		/// Draws a tooltip when hovering over the <paramref name="controlRect"/>
		/// </summary>
		/// <remarks>
		/// Unity doesn't support this by default, so this is kind of a hack.
		/// </remarks>
		/// <param name="controlRect">The rect that will react to show the tooltip.</param>
		/// <param name="text">The text of the tooltip</param>
		public static void DrawControlTooltip(Rect controlRect, string text) {
			if (controlRect.Contains(Event.current.mousePosition)) {

				var content = new GUIContent(text);
				var size = EditorStyles.helpBox.CalcSize(content);
				size.x += 10; // Sometimes it needs more room, since Unity 6
				var rect = new Rect(controlRect.position + Vector2.left * size.x, size);

				// Use the dark color of the editor
				EditorGUI.DrawRect(rect, new Color(0.216f, 0.216f, 0.216f));
				GUI.Box(rect, content, EditorStyles.helpBox);

			}
		}

		/// <summary>
		/// Draws a DropdownButton that creates a hierarchical GenericMenu.
		/// </summary>
		/// <param name="id">
		/// A string that will be used to identify who called this when <paramref name="onNewPath"/> is invoked which happens a few milliseconds later.
		/// </param>
		/// <param name="rect">The rect of the dropdownbutton.</param>
		/// <param name="currentPath">The current path that is selected, for example "Parent/Child/Grandchild"</param>
		/// <param name="allPaths">A list of all the paths that the GenericMenu will show.</param>
		/// <param name="onNewPath">A callback that receives the <paramref name="id"/> and the choosen composite object path.</param>
		public static void HierarchyDropdown(string id, Rect rect, string currentPath, List<string> allPaths, Action<string, string> onNewPath) {

			var buttonContent = new GUIContent(GetLeafName(currentPath));

			if (EditorGUI.DropdownButton(rect, buttonContent, FocusType.Keyboard)) {
				var menu = new GenericMenu();
				foreach (var path in allPaths) {
					var compositeObjectPath = path;
					menu.AddItem(new GUIContent(path), path == currentPath, () => {
						onNewPath?.Invoke(id, compositeObjectPath);
						GUI.changed = true;
					});
				}
				menu.DropDown(rect);
			}

		}

		#endregion


		#region Private Static Methods

		private static string GetLeafName(string path) {
			return string.IsNullOrEmpty(path) ? "Null" : path.Split('/').Last();
		}

		#endregion


	}

}