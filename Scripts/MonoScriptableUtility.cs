namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public static class MonoScriptableUtility {


		#region Public Static Methods

		public static void RecreateMonoScriptableObjects<T>(MonoScriptableField<T>[] fields, Object owner) where T : MonoScriptableObject {
			foreach (var field in fields) {
				RecreateMonoScriptableObject(field, owner);
			}
		}

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

		public static void RecreateRepeatedMonoScriptableItem<T>(MonoScriptableField<T>[] fields, Object owner) where T : MonoScriptableObject {
			
			var fieldIDs = new List<int>();
			
			foreach (var field in fields) {
				if (field.Object != null && !fieldIDs.Contains(field.Object.GetInstanceID())) {
					fieldIDs.Add(field.Object.GetInstanceID());
				} else {
					RecreateMonoScriptableObject(field, owner);
				}
			}

		}

		#endregion


	}

}