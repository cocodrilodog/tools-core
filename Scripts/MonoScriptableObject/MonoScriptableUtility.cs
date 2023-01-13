namespace CocodriloDog.Core {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEngine;

	/// <summary>
	/// Utility methods designed to help in the implementation of the <see cref="IMonoScriptableOwner"/>
	/// interface.
	/// </summary>
	public static class MonoScriptableUtility {


		#region Public Static Methods

		/// <summary>
		/// Recreates all the <see cref="MonoScriptableObject"/> assets referenced by the <paramref name="fields"/> array 
		/// and owned by <paramref name="owner"/>.
		/// </summary>
		/// <typeparam name="T">The type of <see cref="MonoScriptableObject"/></typeparam>
		/// <param name="fields">An array of referencing fields</param>
		/// <param name="owner">The owner of the <see cref="MonoScriptableObject"/> assets</param>
		public static void RecreateMonoScriptableObjects<T>(MonoScriptableField<T>[] fields, UnityEngine.Object owner) where T : MonoScriptableObject {
			foreach (var field in fields) {
				RecreateMonoScriptableObject(field, owner);
			}
		}

		/// <summary>
		/// Recreates a <see cref="MonoScriptableObject"/> asset referenced by the <paramref name="field"/> and owned by 
		/// <paramref name="owner"/>.
		/// </summary>
		/// <typeparam name="T">The type of <see cref="MonoScriptableObject"/></typeparam>
		/// <param name="field">The referencing field</param>
		/// <param name="owner">The owner of the <see cref="MonoScriptableObject"/> asset</param>
		public static void RecreateMonoScriptableObject<T>(MonoScriptableField<T> field, UnityEngine.Object owner) where T : MonoScriptableObject {
			if (field.Object != null) {

				var clone = ScriptableObject.Instantiate(field.Object);
				clone.name = field.Object.name;

				field.SetObject(clone);
				field.Object.SetOwner(owner);
				if (field.Object is IMonoScriptableOwner) {
					((IMonoScriptableOwner)field.Object).OnMonoScriptableOwnerCreated();
				}

			}
		}

		/// <summary>
		/// Recreates the <see cref="MonoScriptableObject"/>s in the provided array <paramref name="fields"/> if they are 
		/// repeated ocurrences in the array.
		/// </summary>
		/// <typeparam name="T">The type of <see cref="MonoScriptableObject"/></typeparam>
		/// <param name="fields">An array of referencing fields</param>
		/// <param name="owner">The owner of the <see cref="MonoScriptableObject"/> assets</param>
		public static void RecreateRepeatedMonoScriptableArrayOrListItems<T>(MonoScriptableField<T>[] fields, UnityEngine.Object owner) where T : MonoScriptableObject {
			
			var fieldIDs = new List<int>();
			
			foreach (var field in fields) {
				if (field.Object != null && !fieldIDs.Contains(field.Object.GetInstanceID())) {
					fieldIDs.Add(field.Object.GetInstanceID());
				} else if (field.Object != null) {
					RecreateMonoScriptableObject(field, owner);
				}
			}

		}

		/// <summary>
		/// Recreates the <see cref="MonoScriptableObject"/>s owned by <paramref name="owner"/> at the specified
		/// <paramref name="propertyPath"/>.
		/// </summary>
		/// <typeparam name="T">The type of <see cref="MonoScriptableObject"/></typeparam>
		/// <param name="propertyPath">The property path as defined by SerializedObjects</param>
		/// <param name="owner">The owner of the <see cref="MonoScriptableObject"/> asset</param>
		public static void RecreateMonoScriptableObjectAtPath<T>(string propertyPath, UnityEngine.Object owner) where T : MonoScriptableObject {

			var pathParts = propertyPath.Split('.');
			object value = owner;

			for (int i = 0; i < pathParts.Length; i++) {

				var fieldInfo = value.GetType().GetField(pathParts[i], BindingFlags);
				
				if (fieldInfo != null) {

					// Recursion
					value = fieldInfo.GetValue(value);

					if (value is MonoScriptableField<T>) {
						// The value is just a MonoScriptableField
						RecreateMonoScriptableObject((value as MonoScriptableField<T>), owner);
						break;
					} else if (value is IList) {

						// The value is either a list or an array of MonoScriptableField
						var valueType = value.GetType();
						var targetElementType = typeof(MonoScriptableField<T>);
						Type valueElementType;

						// Process the index part of the path "data[index]"
						var indexPart = pathParts[pathParts.Length - 2];
						var indexString = string.Empty;
						for (int j = 0; j < indexPart.Length; j++) {
							if (Char.IsDigit(indexPart[j])) {
								indexString += indexPart[j];
							}
						}
						var index = int.Parse(indexString);

						// The value is a List<MonoScriptableField<T>>
						if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(List<>)) {
							valueElementType = value.GetType().GetGenericArguments()[0];
							if (targetElementType.IsAssignableFrom(valueElementType)) {
								RecreateMonoScriptableObject((value as IList)[index] as MonoScriptableField<T>, owner);
								break;
							}
						}

						// The value is MonoScriptableField<T>[]
						if (valueType.IsArray) {
							valueElementType = valueType.GetElementType();
							if (targetElementType.IsAssignableFrom(valueElementType)) {
								RecreateMonoScriptableObject((value as IList)[index] as MonoScriptableField<T>, owner);
								break;
							}
						}

					}

				} else {
					break;
				}

			}

		}

		#endregion

		private static BindingFlags BindingFlags => BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;


	}

}