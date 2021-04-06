namespace CocodriloDog.Core {

	using System;
	using UnityEngine;

	/// <summary>
	/// A range between a <see cref="MinValue"/> and  <see cref="MaxValue"/>.
	/// </summary>
	[Serializable]
	public struct FloatRange {


		#region Public Properties

		public float MinValue {
			get => m_MinValue;
			set {
				if (value > MaxValue) {
					m_MaxValue = value;
				}
				m_MinValue = value;
			}
		}

		public float MaxValue {
			get => m_MaxValue;
			set {
				if (value < MinValue) {
					m_MinValue = value;
				}
				m_MaxValue = value;
			}
		}

		public float Length => MaxValue - MinValue;

		#endregion


		#region Public Constructors

		public FloatRange(float minValue, float maxValue) {
			m_MinValue = minValue;
			m_MaxValue = maxValue;
			if(m_MaxValue < m_MinValue) {
				m_MinValue = m_MaxValue;
			}
		}

		#endregion


		#region Public Methods

		public override string ToString() {
			return string.Format("({0}, {1})", MinValue, MaxValue);
		}

		#endregion


		#region Private Properties

		[SerializeField]
		private float m_MinValue;

		[SerializeField]
		private float m_MaxValue;

		#endregion


	}

}
