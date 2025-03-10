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
					m_Asset.Value = value; // This triggers the asset value change events
				} else {

					var raiseChangeEvent = value != m_Value;
					var previousValue = Value;

					if(raiseChangeEvent && previousValue != null) {
						_OnOldReference?.Invoke(previousValue);
					}

					m_Value = value;

					if (raiseChangeEvent) {

						_OnValueChange?.Invoke(previousValue, m_Value);

						if (m_Value != null) {
							_NowOrOnNewReference?.Invoke(m_Value);
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
					_OnValueChange += value;
					m_AssetValueChangeHandlers[value] = (pv, nv) => value?.Invoke(pv as T, nv as T);
					m_Asset.OnValueChange += m_AssetValueChangeHandlers[value];
				}
			}
			remove {
				lock (this) {
					_OnValueChange -= value;
					m_Asset.OnValueChange -= m_AssetValueChangeHandlers[value];
				}
			}
		}

		/// <inheritdoc cref="ScriptableReference.NowOrOnNewReference"/>
		public event ReferenceChange NowOrOnNewReference {
			add {
				lock (this) {
					
					_NowOrOnNewReference += value;
					m_AssetReferenceChangeHandlers[value] = v => value?.Invoke(v as T);
					m_Asset.NowOrOnNewReference += m_AssetReferenceChangeHandlers[value];

					if (!UseAsset) {
						if (Value != null) {
							_NowOrOnNewReference?.Invoke(m_Value);
						}
					}

				}
			}
			remove {
				lock (this) {
					_NowOrOnNewReference -= value;
					m_Asset.NowOrOnNewReference -= m_AssetReferenceChangeHandlers[value];
				}
			}
		}

		/// <inheritdoc cref="ScriptableReference.OnOldReference"/>
		public event ReferenceChange OnOldReference {
			add {
				lock (this) {
					_OnOldReference += value;
					m_AssetReferenceChangeHandlers[value] = v => value?.Invoke(v as T);
					m_Asset.OnOldReference += m_AssetReferenceChangeHandlers[value];
				}
			}
			remove {
				lock (this) {
					_OnOldReference -= value;
					m_Asset.OnOldReference -= m_AssetReferenceChangeHandlers[value];
				}
			}
		}

		#endregion


		#region Private Fields - Serialized

		[SerializeField]
		private T m_Value;

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

		private event ReferenceChange _NowOrOnNewReference;

		private event ReferenceChange _OnOldReference;

		#endregion


	}

}