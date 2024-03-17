namespace CocodriloDog.Core {

	using System;
	using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ScriptableValue<T> : ScriptableObject {


        #region Public Properties

        public virtual T Value {
            get => m_Value;
            set {
                var previousValue = m_Value;
                m_Value = value;
                OnValueChange?.Invoke(previousValue);
            }
        }

		#endregion


		#region Pubic Delegates

		public delegate void ValueChange(T previousValue);

		#endregion


		#region Public Events

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
