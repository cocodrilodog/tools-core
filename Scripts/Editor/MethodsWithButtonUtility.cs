namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEngine;

	/// <summary>
	/// Utility to collect, store and retrieve <see cref="MethodInfo"/>s of methods that use the 
	/// <see cref="ButtonAttribute"/>.
	/// </summary>
	public static class MethodsWithButtonUtility {


		#region Public Static Methods

		/// <summary>
		/// Gets a dictionary of <see cref="MethodInfo"/>s that correspond to methods that used the <see cref="ButtonAttribute"/>.
		/// The <see cref="MethodInfo"/>s are keyed by index, where the index was defined in the attribute usage.
		/// </summary>
		/// <param name="type">The type that will be inspected to collect the <see cref="MethodInfo"/>s</param>
		/// <returns>The dictionary of <see cref="MethodInfo"/>s, keyed by index.</returns>
		public static Dictionary<int, List<MethodInfo>> GetMethodsWithButtonByIndex(Type type) {

			if (s_MethodsWithButtonByType.TryAdd(type, new Dictionary<int, List<MethodInfo>>())) {

				// Store methods that have ButtonAttribute here
				var methodsWithButtonByIndex = new Dictionary<int, List<MethodInfo>>();

				// Get all methods of the target object
				MethodInfo[] allMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

				// If the method has the ButtonAttribute, store it
				foreach (MethodInfo method in allMethods) {
					var buttonAttribute = System.Attribute.GetCustomAttribute(method, typeof(ButtonAttribute)) as ButtonAttribute;
					if (buttonAttribute != null) {
						// The attribute may be used more than once with the same index, so we store
						// the methods in a list
						methodsWithButtonByIndex.TryAdd(buttonAttribute.Index, new List<MethodInfo>());
						methodsWithButtonByIndex[buttonAttribute.Index].Add(method);
					}
				}

				s_MethodsWithButtonByType[type] = methodsWithButtonByIndex;

			}
			return s_MethodsWithButtonByType[type];
		}

		#endregion


		#region Private Static Fields

		private static Dictionary<Type, Dictionary<int, List<MethodInfo>>> s_MethodsWithButtonByType =
			new Dictionary<Type, Dictionary<int, List<MethodInfo>>>();

		#endregion


	}

}