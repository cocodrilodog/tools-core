namespace CocodriloDog.Core {

	using UnityEditor;
	using UnityEngine;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public enum PathStyle {
		SeparatedBySlash,
		SeparatedByPoint,
		OnlyLastStep
	}

	public static class CDEditorGUI {


		#region Public Static Methods

		/// <summary>
		/// Draws a tooltip when hovering over the <paramref name="controlRect"/> to the left side of the 
		/// <paramref name="controlRect"/>.
		/// </summary>
		/// <remarks>
		/// Unity doesn't support this by default, so this is kind of a hack.
		/// </remarks>
		/// <param name="controlRect">The rect that will react to show the tooltip.</param>
		/// <param name="text">The text of the tooltip</param>
		/// <param name="positionOffset">Offset with respect to the default position of the tooltip.</param>
		public static void DrawControlTooltip(Rect controlRect, string text, Vector2 positionOffset = default) {
			if (controlRect.Contains(Event.current.mousePosition)) {

				var content = new GUIContent(text);

				var size = EditorStyles.helpBox.CalcSize(content);
				size.x += 10; // Sometimes it needs more room, since Unity 6
				
				var position = controlRect.position + Vector2.left * size.x;
				position += positionOffset;
				
				var rect = new Rect(position, size);

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
		/// A good option for this parameter is the property path of the relevant property.
		/// </param>
		/// <param name="rect">The rect of the dropdownbutton.</param>
		/// <param name="currentPath">The current path that is selected, for example "Parent/Child/Grandchild"</param>
		/// <param name="pathStyle">The style to display the path in the button.</param>
		/// <param name="defaultPath">The path that is shown in the dropdpwn button whrn no path is selected.</param>
		/// <param name="allPaths">A list of all the paths that the GenericMenu will show. A path "--" will create a horizontal separator</param>
		/// <param name="onNewPath">A callback that receives the <paramref name="id"/> and the choosen composite object path.</param>
		public static void HierarchyDropdown(string id, Rect rect, string currentPath, PathStyle pathStyle, string defaultPath, List<string> allPaths, Action<string, string> onNewPath) {

			string buttonText = string.IsNullOrEmpty(currentPath) ? defaultPath : pathStyle switch {
				PathStyle.SeparatedBySlash => currentPath,
				PathStyle.SeparatedByPoint => currentPath.Replace("/", "."),
				PathStyle.OnlyLastStep => GetLeafName(currentPath, defaultPath),
				_ => defaultPath,
			};

			if (EditorGUI.DropdownButton(rect, new GUIContent(buttonText), FocusType.Keyboard)) {
				var menu = new GenericMenu();
				foreach (var path in allPaths) {
					var compositeObjectPath = path;
					if (path == "--") {
						menu.AddSeparator("");
					} else {
						menu.AddItem(new GUIContent(path), path == currentPath, () => {
							onNewPath?.Invoke(id, compositeObjectPath);
							GUI.changed = true;
						});
					}
				}
				menu.DropDown(rect);
			}

		}

		#endregion


		#region Private Static Methods

		private static string GetLeafName(string path, string defaultLeaf) {
			return string.IsNullOrEmpty(path) ? defaultLeaf : path.Split('/').Last();
		}

		#endregion


	}

}