namespace CocodriloDog.Core {

	using System;
	using UnityEngine;

	/// <summary>
	/// An <c>int</c> range between a <see cref="MinValue"/> and  <see cref="MaxValue"/>.
	/// </summary>
	[Serializable]
	public class MinMaxInt {


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


		#region Private Properties

		[SerializeField]
		private int m_MinValue;

		[SerializeField]
		private int m_MaxValue;

		#endregion


	}

}