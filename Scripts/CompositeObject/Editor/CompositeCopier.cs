namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A singleton class used to copy and paste <see cref="Core.CompositeObject"/>s.
	/// </summary>
	/// 
	/// <remarks>
	/// By using a ScriptableObject as the base class, we leverage the cloning capabilities of 
	/// <see cref="UnityEngine.Object.Instantiate(UnityEngine.Object)"/>
	/// </remarks>
	public class CompositeCopier : ScriptableObject {


		#region Public Static Properties

		/// <summary>
		/// The copied <see cref="CompositeObject"/>.
		/// </summary>
		public static CompositeObject CopiedObject => Instance.CompositeObject;

		/// <summary>
		/// The <see cref="Type"/> of the last copied object, Used to validate against the paste target field.
		/// </summary>
		public static Type CopiedType => Instance.CompositeObject?.GetType();

		#endregion


		#region Public Static Methods

		/// <summary>
		/// Copies the provided <paramref name="obj"/> and stores a reference to it until it is pasted.
		/// </summary>
		/// <param name="obj"></param>
		public static void Copy(CompositeObject obj) {
			Instance.CompositeObject = obj;
		}

		/// <summary>
		/// Gets a copy of the last copied <see cref="CompositeObject"/>.
		/// </summary>
		/// <returns>The copy</returns>
		public static CompositeObject Paste() {
			var clone = Instantiate(Instance).CompositeObject;
			Instance.CompositeObject = null;
			return clone;
		}

		#endregion


		#region Private Static Fields

		private static CompositeCopier s_CompositeObject;

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


	}

}