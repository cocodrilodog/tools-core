namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Allows to choose between a value assigned on the MonoBehaviour inspector or a <see cref="ScriptableValue{T}"/>
	/// and returns either the value on the inspector or the value on the <see cref="ScriptableValue{T}"/> asset, 
	/// depending on <see cref="ScriptableFieldBase.UseAsset"/>.
	/// </summary>
	/// <typeparam name="T">The type of value expected.</typeparam>
	[Serializable]
	public class ScriptableValueField<T> : ScriptableFieldBase {


		#region Public Properties

		/// <summary>
		/// Either the value on the inspector or the value on the <see cref="ScriptableValue{T}"/> asset, 
		/// depending on <see cref="ScriptableFieldBase.UseAsset"/>.
		/// </summary>
		public T Value {
			get {
				if (UseAsset && m_Asset != null) {
					return m_Asset.Value;
				} else {
					return m_Value;
				}
			}
			set {
				if (UseAsset && m_Asset != null) {
					m_Asset.Value = value;  // This triggers the asset value change event
				} else {
					if (typeof(object).IsAssignableFrom(typeof(T))) { // Reference type
						var valueAsObject = value as object;
						if (valueAsObject != (m_Value as object)) {
							var previousValue = m_Value;
							m_Value = value;
							_OnValueChange?.Invoke(previousValue);
						}
					} else { // Value type
						if (!value.Equals(m_Value)) {
							var previousValue = m_Value;
							m_Value = value;
							_OnValueChange?.Invoke(previousValue);
						}
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
					m_AssetEventHandlers[value] = pv => value?.Invoke(pv);
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

		[CreateAsset]
		[SerializeField]
		private ScriptableValue<T> m_Asset;

		#endregion


		#region Private Fields - Non Serialized

		private Dictionary<ValueChange, ScriptableValue<T>.ValueChange> m_AssetEventHandlers =
			new Dictionary<ValueChange, ScriptableValue<T>.ValueChange>();

		#endregion


		#region Private Events

		private ValueChange _OnValueChange;

		#endregion


	}

}