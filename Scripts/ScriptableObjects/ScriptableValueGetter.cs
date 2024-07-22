namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Allows to choose between a value assigned on the MonoBehaviour inspector or a <see cref="ScriptableValue{T}"/>
	/// and returns either the value on the inspector or the value on the <see cref="ScriptableValue{T}"/> asset, 
	/// depending on <see cref="ScriptableGetterBase.UseAsset"/>.
	/// </summary>
	/// <typeparam name="T">The type of value expected.</typeparam>
	[Serializable]
	public class ScriptableValueGetter<T> : ScriptableGetterBase {


		#region Public Properties

		/// <summary>
		/// Either the value on the inspector or the value on the <see cref="ScriptableValue{T}"/> asset, 
		/// depending on <see cref="ScriptableGetterBase.UseAsset"/>.
		/// </summary>
		public T Value {
			get {
				if (UseAsset && m_Asset != null) {
					return m_Asset.Value;
				} else {
					return m_Value;
				}
			}
		}


		/// <summary>
		/// A reference to the <see cref="ScriptableValue<T>"/> asset.
		/// </summary>
		public ScriptableValue<T> Asset => m_Asset;

		#endregion


		#region Private Fields

		[SerializeField]
		private T m_Value;

		[SerializeField]
		private ScriptableValue<T> m_Asset;

		#endregion


	}

}