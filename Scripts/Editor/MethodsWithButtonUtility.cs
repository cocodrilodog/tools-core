namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Utility that supports the <see cref="ButtonAttribute"/>.
	/// </summary>
	public static class MethodsWithButtonUtility {


		#region Public Static Methods

		/// <summary>
		/// Gets a dictionary of <see cref="MethodInfo"/>s that correspond to methods that used the <see cref="ButtonAttribute"/>.
		/// The <see cref="MethodInfo"/>s are keyed by index, where the index was defined in the attribute usage.
		/// </summary>
		/// <param name="type">The type that will be inspected to collect the <see cref="MethodInfo"/>s</param>
		/// <returns>The dictionary of <see cref="MethodInfo"/>s, keyed by index.</returns>
		public static Dictionary<int, List<ButtonMethodInfo>> GetMethodsWithButtonByIndex(Type type) {

			if (s_MethodsWithButtonByType.TryAdd(type, new Dictionary<int, List<ButtonMethodInfo>>())) {

				// Store methods that have ButtonAttribute here
				var methodsWithButtonByIndex = new Dictionary<int, List<ButtonMethodInfo>>();

				// Get all methods of the target object
				MethodInfo[] allMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

				// If the method has the ButtonAttribute, store it
				foreach (MethodInfo method in allMethods) {
					var buttonAttribute = System.Attribute.GetCustomAttribute(method, typeof(ButtonAttribute)) as ButtonAttribute;
					if (buttonAttribute != null) {
						// The attribute may be used more than once with the same index, so we store
						// the methods in a list
						methodsWithButtonByIndex.TryAdd(buttonAttribute.Index, new List<ButtonMethodInfo>());
						var buttonMethodInfo = new ButtonMethodInfo(
							method,
							buttonAttribute.HorizontalizeSameIndex,
							buttonAttribute.DisableInEditMode,
							buttonAttribute.DisableInPlayMode
						);
						methodsWithButtonByIndex[buttonAttribute.Index].Add(buttonMethodInfo);
					}
				}

				s_MethodsWithButtonByType[type] = methodsWithButtonByIndex;

			}
			return s_MethodsWithButtonByType[type];
		}

		/// <summary>
		/// Gets the accumulated height of the method buttons that correspond to an index within the list of 
		/// properties of a cd object or composite object.
		/// </summary>
		/// <param name="propertyIndex">The index of the property, used to define the place of the button.</param>
		/// <param name="methodOwnerType">The type of the cd object or composite object.</param>
		/// <returns></returns>
		public static float GetMethodButtonsHeightAtPropertyIndex(int propertyIndex, Type methodOwnerType) {
			
			var height = 0f;

			// Add the height of the buttons of the methods with button
			var methodsWithButtonByIndex = MethodsWithButtonUtility.GetMethodsWithButtonByIndex(methodOwnerType);
			if (methodsWithButtonByIndex.TryGetValue(propertyIndex, out var buttonMethodInfos)) {
				var horizontalizeSameIndex = buttonMethodInfos.Any(bmi => bmi.HorizontalizeSameIndex);
				if (horizontalizeSameIndex) {
					// Add only one line
					height += EditorGUIUtility.singleLineHeight + 2;
				} else {
					foreach (var buttonMethodInfo in buttonMethodInfos) {
						height += EditorGUIUtility.singleLineHeight + 2;
					}
				}
			}

			return height;

		}

		/// <summary>
		/// Draws the method buttons that correspond to an index within the list of properties of a cd 
		/// object or composite object.
		/// </summary>
		/// <param name="propertyIndex">The index of the property, used to define the place of the button.</param>
		/// <param name="methodOwnerType">The type of the cd object or composite object.</param>
		/// <param name="methodOwner">The cd object or composite object</param>
		/// <param name="getNextPosition">
		/// Access to the <see cref="PropertyDrawerBase.GetNextPosition(float)"/> method.
		/// </param>
		public static bool DrawMethodButtonsAtPropertyIndex(
			int propertyIndex, Type methodOwnerType, System.Object methodOwner, Func<Rect> getNextPosition
		) {

			var methodsWithButtonByIndex = GetMethodsWithButtonByIndex(methodOwnerType);

			// Draw method buttons if they are on this index
			if (methodsWithButtonByIndex.TryGetValue(propertyIndex, out var buttonMethodInfos)) {

				var horizontalizeSameIndex = buttonMethodInfos.Any(bmi => bmi.HorizontalizeSameIndex);
				Rect horizontalBox = default;
				if (horizontalizeSameIndex) {
					horizontalBox = getNextPosition();
				}

				for (int i = 0; i < buttonMethodInfos.Count; i++) {

					var buttonMethodInfo = buttonMethodInfos[i];
					Rect buttonRect = default;

					if (horizontalizeSameIndex) {
						horizontalBox.xMin += EditorGUI.indentLevel * 15;
						buttonRect = horizontalBox;
						buttonRect.width = horizontalBox.width / buttonMethodInfos.Count;
						buttonRect.x = horizontalBox.x + buttonRect.width * i;
					} else {
						buttonRect = getNextPosition();
						buttonRect.xMin += EditorGUI.indentLevel * 15;
					}

					EditorGUI.BeginDisabledGroup(
						Application.isPlaying ? buttonMethodInfo.DisableInPlayMode : buttonMethodInfo.DisableInEditMode
					);
					if (GUI.Button(buttonRect, ObjectNames.NicifyVariableName(buttonMethodInfo.Method.Name))) {
						buttonMethodInfo.Method.Invoke(methodOwner, null);
					}
					EditorGUI.EndDisabledGroup();

				}

				return true;

			}
			return false;
		}

		#endregion


		#region Private Static Fields

		private static Dictionary<Type, Dictionary<int, List<ButtonMethodInfo>>> s_MethodsWithButtonByType =
			new Dictionary<Type, Dictionary<int, List<ButtonMethodInfo>>>();

		#endregion


	}


	#region Support Classes

	/// <summary>
	/// This class combines the <c>MethodInfo </c> with the properties of the button attribute.
	/// </summary>
	public class ButtonMethodInfo {

		public MethodInfo Method { get; private set; }

		public bool HorizontalizeSameIndex { get; private set; }

		public bool DisableInEditMode { get; private set; }

		public bool DisableInPlayMode { get; private set; }

		public ButtonMethodInfo(
			MethodInfo method, 
			bool horizontalizeSameIndex, 
			bool disableInEditMode, 
			bool disableInPlayMode
		) {
			Method = method;
			HorizontalizeSameIndex = horizontalizeSameIndex;
			DisableInEditMode = disableInEditMode;
			DisableInPlayMode = disableInPlayMode;
		}

	}

	#endregion


}