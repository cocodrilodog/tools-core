namespace CocodriloDog.Core {

	using System;
	using UnityEngine;

	/// <summary>
	/// A range between a <see cref="MinValue"/> and  <see cref="MaxValue"/>.
	/// </summary>
	[Serializable]
	public class FloatRange {


		#region Public Properties

		public float MinValue {
			get { return m_MinValue; }
			set {
				if (value > MaxValue) {
					m_MaxValue = value;
				}
				m_MinValue = value;
			}
		}

		public float MaxValue {
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
		private float m_MinValue;

		[SerializeField]
		private float m_MaxValue;

		#endregion


	}

}
