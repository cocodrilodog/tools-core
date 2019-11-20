namespace CocodriloDog.Core {

	using UnityEngine;

	/// <summary>
	/// Used as a Unity Layer Field where only one layer can be chosen.
	/// </summary>
	[System.Serializable]
	public class SingleUnityLayer {


		#region Public Properties

		public int LayerIndex {
			get { return m_LayerIndex; }
			set {
				if (value > 0 && value < 32) {
					m_LayerIndex = value;
				}
			}
		}

		public int Mask {
			get { return 1 << m_LayerIndex; }
		}

		#endregion


		#region Private Fields

		[SerializeField]
		private int m_LayerIndex = 0;

		#endregion


	}

}