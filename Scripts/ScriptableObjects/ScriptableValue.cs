namespace CocodriloDog.Core {

	using System;
	using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

	/// <summary>
	/// A ScriptableObject that stores/references a value of type <typeparamref name="T"/>.
	/// </summary>
	/// 
	/// <remarks>
	/// This is designed to store values or references to objects at a project level so that
	/// they can be easily retrieved by other objects without relying on a forced 
	/// singleton pattern.
	/// </remarks>
	/// 
	/// <typeparam name="T">The type to store.</typeparam>
    public class ScriptableValue<T> : ScriptableObject {


        #region Public Properties

		/// <summary>
		/// The value that is stored/referenced by this asset.
		/// </summary>
        public virtual T Value {
            get => m_Value;
            set {
				if (typeof(object).IsAssignableFrom(typeof(T))) { // Reference type
					var valueAsObject = value as object;
					if (valueAsObject != (m_Value as object)) {
						var previousValue = m_Value;
						m_Value = value;
						OnValueChange?.Invoke(previousValue);
					}
				} else { // Value type
					if (!value.Equals(m_Value)) {
						var previousValue = m_Value;
						m_Value = value;
						OnValueChange?.Invoke(previousValue);
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
		public ValueChange OnValueChange;

		#endregion


		#region Unity Methods

		private void OnDestroy() => OnValueChange = null;

		#endregion


		#region Private Fields

		[SerializeField]
        private T m_Value;

        #endregion


    }

}
