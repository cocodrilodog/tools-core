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
	/// <typeparam name="T">The type of object expected.</typeparam>
	/// <typeparam name="TField">The type of the <see cref="InterfaceField{T}"/></typeparam>
	[Serializable]
	public class ScriptableInterfaceField<T> : ScriptableFieldBase where T : class {


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
					return m_Value.Value;
				}
			}
			set {
				if (UseAsset && m_Asset != null) {
					m_Asset.Value = value as UnityEngine.Object; // This triggers the asset value change event
				} else {

					var previousValue = Value;
					var valueChanges = value != previousValue;

					if (valueChanges && previousValue != null) {
						_OnInstanceDiscard?.Invoke(previousValue);
					}

					if (valueChanges) {

						m_Value.Value = value;
						_OnValueChange?.Invoke(previousValue, Value);

						if (m_Value != null) {
							_OnInstanceReady?.Invoke(m_Value as T);
						}

					}

				}
			}
		}

		#endregion


		#region Pubic Delegates

		public delegate void ValueChange(T previousValue, T newValue);

		public delegate void ReferenceChange(T unityObject);

		#endregion


		#region Public Events

		/// <summary>
		/// Raised when the <see cref="Value"/> changes.
		/// </summary>
		public event ValueChange OnValueChange {
			add {
				lock (this) {
					if (UseAsset && m_Asset != null) {
						m_AssetValueChangeHandlers[value] = (pv, nv) => value?.Invoke(pv as T, nv as T);
						m_Asset.OnValueChange += m_AssetValueChangeHandlers[value];
					} else {
						_OnValueChange += value;
					}
				}
			}
			remove {
				lock (this) {
					if (UseAsset && m_Asset != null) {
						m_Asset.OnValueChange -= m_AssetValueChangeHandlers[value];
					} else {
						_OnValueChange -= value;
					}
				}
			}
		}

		/// <inheritdoc cref="ScriptableReference.OnInstanceReady"/>
		public event ReferenceChange OnInstanceReady {
			add {
				lock (this) {
					if (UseAsset && m_Asset != null) {
						m_AssetReferenceChangeHandlers[value] = v => value?.Invoke(v as T);
						m_Asset.OnInstanceReady += m_AssetReferenceChangeHandlers[value];
					} else {
						_OnInstanceReady += value;
					}
				}
			}
			remove {
				lock (this) {
					if (UseAsset && m_Asset != null) {
						m_Asset.OnInstanceReady -= m_AssetReferenceChangeHandlers[value];
					} else {
						_OnInstanceReady -= value;
					}
				}
			}
		}

		/// <inheritdoc cref="ScriptableReference.OnInstanceDiscard"/>
		public event ReferenceChange OnInstanceDiscard {
			add {
				lock (this) {
					if (UseAsset && m_Asset != null) {
						m_AssetReferenceChangeHandlers[value] = v => value?.Invoke(v as T);
						m_Asset.OnInstanceDiscard += m_AssetReferenceChangeHandlers[value];
					} else {
						_OnInstanceDiscard += value;
					}
				}
			}
			remove {
				lock (this) {
					if (UseAsset && m_Asset != null) {
						m_Asset.OnInstanceDiscard -= m_AssetReferenceChangeHandlers[value];
					} else {
						_OnInstanceDiscard -= value;
					}
				}
			}
		}

		#endregion


		#region Private Fields - Serialized

		[SerializeField]
		private InterfaceField<T> m_Value;

		[CreateAsset]
		[SerializeField]
		private ScriptableReference m_Asset;

		#endregion


		#region Private Fields - Non Serialized

		private Dictionary<ValueChange, ScriptableReference.ValueChange> m_AssetValueChangeHandlers = new();

		private Dictionary<ReferenceChange, ScriptableReference.ReferenceChange> m_AssetReferenceChangeHandlers = new();

		#endregion


		#region Private Events

		private event ValueChange _OnValueChange;

		private event ReferenceChange _OnInstanceReady;

		private event ReferenceChange _OnInstanceDiscard;

		#endregion


	}

}