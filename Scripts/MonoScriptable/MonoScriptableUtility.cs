namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
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
		public static void RecreateMonoScriptableObjects<T>(MonoScriptableField<T>[] fields, Object owner) where T : MonoScriptableObject {
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
		public static void RecreateMonoScriptableObject<T>(MonoScriptableField<T> field, Object owner) where T : MonoScriptableObject {
			if (field.Object != null) {

				var clone = ScriptableObject.Instantiate(field.Object);
				clone.name = field.Object.name;

				field.SetObject(clone);
				field.Object.SetOwner(owner);
				if (field.Object is IMonoScriptableOwner) {
					((IMonoScriptableOwner)field.Object).RecreateMonoScriptableObjects();
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
		public static void RecreateRepeatedMonoScriptableArrayOrListItems<T>(MonoScriptableField<T>[] fields, Object owner) where T : MonoScriptableObject {
			
			var fieldIDs = new List<int>();
			
			foreach (var field in fields) {
				if (field.Object != null && !fieldIDs.Contains(field.Object.GetInstanceID())) {
					fieldIDs.Add(field.Object.GetInstanceID());
				} else if (field.Object != null) {
					RecreateMonoScriptableObject(field, owner);
				}
			}

		}

		#endregion


	}

}