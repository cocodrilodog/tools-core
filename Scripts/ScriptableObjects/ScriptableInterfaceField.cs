namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Allows to choose between a reference of an interface object on the scene or a <see cref="ScriptableReference"/>
	/// and returns either the object from the scene or the value of the <see cref="ScriptableReference"/> asset, 
	/// depending on <see cref="ScriptableFieldBase.UseAsset"/>.
	/// </summary>
	/// <typeparam name="TValue">The type of object expected.</typeparam>
	/// <typeparam name="TField">The type of the <see cref="InterfaceField{T}"/></typeparam>
	[Serializable]
	public class ScriptableInterfaceField<TValue, TField> : ScriptableFieldBase 
		where TValue : class
		where TField : InterfaceField<TValue> {


		#region Public Properties

		/// <summary>
		/// Gets and sets either the object from the scene or the value of the <see cref="ScriptableReference"/>
		/// asset, depending on <see cref="ScriptableFieldBase.UseAsset"/>.
		/// </summary>
		public TValue Value {
			get {
				if (UseAsset && m_Asset != null) {
					return m_Asset.Value as TValue;
				} else {
					return m_Value.Value;
				}
			}
			set {
				if (UseAsset && m_Asset != null) {
					m_Asset.Value = value as UnityEngine.Object; // This triggers the asset value change event
				} else {
					var previousValue = Value;
					m_Value.Value = value;
					if (m_Value != previousValue) {
						_OnValueChange?.Invoke(previousValue);
					}
				}
			}
		}

		#endregion


		#region Pubic Delegates

		public delegate void ValueChange(TValue previousValue);

		#endregion


		#region Public Events

		/// <summary>
		/// Raised when the <see cref="Value"/> changes.
		/// </summary>
		public event ValueChange OnValueChange {
			add {
				lock (this) {
					_OnValueChange += value;
					m_AssetEventHandlers[value] = pv => value?.Invoke(pv as TValue);
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
		private TField m_Value;

		[CreateAsset]
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