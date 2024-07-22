namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Allows to choose between a reference of an object on the scene or a <see cref="ScriptableReference"/>
	/// and returns either the object from the scene or the value of the <see cref="ScriptableReference"/> asset, 
	/// depending on <see cref="ScriptableGetterBase.UseAsset"/>.
	/// </summary>
	/// <typeparam name="T">The type of object expected.</typeparam>
	[Serializable]
	public class ScriptableReferenceGetter<T> : ScriptableGetterBase where T : UnityEngine.Object {


		#region Public Properties

		/// <summary>
		/// Either the object from the scene or the value of the <see cref="ScriptableReference"/> asset, 
		/// depending on <see cref="ScriptableGetterBase.UseAsset"/>.
		/// </summary>
		public T Value {
			get {
				if (UseAsset && m_Asset != null) {
					var result = m_Asset.Value as T;
					if (result == null) {
						Debug.LogWarning($"Object {m_Asset.Value} can not be returned as {typeof(T).Name}");
					}
					return m_Asset.Value as T;
				} else {
					return m_Value;
				}
			}
		}

		/// <summary>
		/// A reference to the <see cref="ScriptableReference"/> asset.
		/// </summary>
		public ScriptableReference Asset => m_Asset;

		#endregion


		#region Private Fields

		[SerializeField]
		private T m_Value;

		[SerializeField]
		private ScriptableReference m_Asset;

		#endregion


	}

}