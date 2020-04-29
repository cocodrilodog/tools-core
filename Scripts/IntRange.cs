namespace CocodriloDog.Core {

	using System;
	using UnityEngine;

	/// <summary>
	/// An <c>int</c> range between a <see cref="MinValue"/> and  <see cref="MaxValue"/>.
	/// </summary>
	[Serializable]
	public struct IntRange {


		#region Public Properties

		public int MinValue {
			get { return m_MinValue; }
			set {
				if (value > MaxValue) {
					m_MaxValue = value;
				}
				m_MinValue = value;
			}
		}

		public int MaxValue {
			get { return m_MaxValue; }
			set {
				if (value < MinValue) {
					m_MinValue = value;
				}
				m_MaxValue = value;
			}
		}

		#endregion


		#region Public Constructors

		public IntRange(int minValue, int maxValue) {
			m_MinValue = minValue;
			m_MaxValue = maxValue;
			if (m_MaxValue < m_MinValue) {
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
		private int m_MinValue;

		[SerializeField]
		private int m_MaxValue;

		#endregion


	}

}