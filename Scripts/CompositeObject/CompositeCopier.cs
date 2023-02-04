namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class CompositeCopier : ScriptableObject {


		public static void Copy(CompositeObject obj) {
			Instance.CompositeObject = obj;
			Debug.Log($"Copied: {Instance.CompositeObject.Name}");
		}

		public static Type CopiedType => Instance.CompositeObject?.GetType();

		public static CompositeObject Paste() {
			var clone = Instantiate(Instance).CompositeObject;
			Debug.Log($"Pasted: {Instance.CompositeObject.Name}");
			Instance.CompositeObject = null;
			return clone;
		}

		#region Private Static Fields

		private static CompositeCopier s_CompositeObject;

		#endregion


		#region Private Fields

		[SerializeReference]
		private CompositeObject m_CompositeObject;

		#endregion


		#region Private Properties

		private CompositeObject CompositeObject {
			get => m_CompositeObject;
			set => m_CompositeObject = value;
		}

		#endregion


		#region Private Static Properties

		private static CompositeCopier Instance {
			get {
				if (s_CompositeObject == null) {
					s_CompositeObject = ScriptableObject.CreateInstance<CompositeCopier>();
				}
				return s_CompositeObject;
			}
		}

		#endregion


	}

}