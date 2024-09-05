namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Allows to choose between a reference of an object on the scene or a <see cref="ScriptableReference"/>
	/// and returns either the object from the scene or the value of the <see cref="ScriptableReference"/> asset, 
	/// depending on <see cref="ScriptableFieldBase.UseAsset"/>.
	/// </summary>
	/// <typeparam name="T">The type of object expected.</typeparam>
	[Serializable]
	public class ScriptableReferenceField<T> : ScriptableFieldBase where T : UnityEngine.Object {


		#region Public Properties

		/// <summary>
		/// Gets and sets either the object from the scene or the value of the <see cref="ScriptableReference"/>
		/// asset, depending on <see cref="ScriptableFieldBase.UseAsset"/>.
		/// </summary>
		public T Value {
			get {
				if (UseAsset && m_Asset != null) {
					return m_Asset.Value as T;
				} else {
					return m_Value;
				}
			}
			set {
				if (UseAsset && m_Asset != null) {
					m_Asset.Value = value; // This triggers the asset value change event
				} else {
					var prevValue = Value;
					m_Value = value;
					if (m_Value != prevValue) {
						_OnValueChange?.Invoke(prevValue);
					}
				}
			}
		}

		#endregion


		#region Pubic Delegates

		public delegate void ValueChange(T previousValue);

		#endregion


		#region Public Events

		/// <summary>
		/// Raised when the <see cref="Value"/> changes.
		/// </summary>
		public event ValueChange OnValueChange {
			add {
				lock (this) {
					_OnValueChange += value;
					m_AssetEventHandlers[value] = pv => value?.Invoke(pv as T);
					m_Asset.OnValueChange += m_AssetEventHandlers[value];
				}
			}
			remove {
				lock (this) {
					_OnValueChange -= value;
					m_Asset.OnValueChange -= m_AssetEventHandlers[value];
				}
			}
		}

		#endregion


		#region Private Fields - Serialized

		[SerializeField]
		private T m_Value;

		[SerializeField]
		private ScriptableReference m_Asset;

		#endregion


		#region Private Fields - Non Serialized

		private Dictionary<ValueChange, ScriptableReference.ValueChange> m_AssetEventHandlers = 
			new Dictionary<ValueChange, ScriptableReference.ValueChange>();

		#endregion


		#region Private Events

		private ValueChange _OnValueChange;

		#endregion


	}

}