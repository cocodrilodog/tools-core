namespace CocodriloDog.Core {

	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Attribute that allows to edit a <see cref="MinMaxRange"/> float with a 
	/// MinMaxSlider.
	/// </summary>
	public class MinMaxRangeAttribute : PropertyAttribute {


		#region Public Fields

		public float MinLimit;

		public float MaxLimit;

		#endregion


		#region Constructors

		public MinMaxRangeAttribute(float minLimit, float maxLimit) {
			MinLimit = minLimit;
			MaxLimit = maxLimit;
		}

		#endregion


	}

	/// <summary>
	/// A range between a <see cref="MinValue"/> and  <see cref="MaxValue"/>.
	/// </summary>
	[Serializable]
	public class MinMaxRange {

		public float MinValue {
			get { return m_MinValue; }
			set { 
				if(value > m_MaxValue) {
					value = m_MaxValue;
				}
				m_MinValue = value;
			}
		}

		public float MaxValue {
			get { return m_MaxValue; }
			set {
				if (value < m_MinValue) {
					value = m_MinValue;
				}
				m_MaxValue = value;
			}
		}

		[SerializeField]
		public float m_MinValue;

		[SerializeField]
		public float m_MaxValue;

	}

}