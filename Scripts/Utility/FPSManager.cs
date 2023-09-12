namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;

	// TODO: This may be a good candidate for a `Settings` package
	public class FPSManager : MonoBehaviour {


		#region Unity Methods

		private void Awake() {
			DontDestroyOnLoad(gameObject);
			var settings = PlatformSettings.FirstOrDefault(s => s.Platform == Application.platform);
			if (settings != null) {
				QualitySettings.vSyncCount = settings.VSyncCount;
				Application.targetFrameRate = settings.TargetFrameRate;
			}
		}

		private void Update() {
			if (MonitorFPS) {
				m_FPS = (int)(1 / Time.deltaTime);
			}
		}

		void OnGUI() {
			if (MonitorFPS) {
				//Display the fps and round to 2 decimals
				GUI.Label(new Rect(5, 5, 100, 25), m_FPS.ToString("F2") + " FPS");
			}
		}

		#endregion


		#region Private Fields - Serialized

		[SerializeField]
		private bool m_MonitorFPS;

		[SerializeField]
		private List<FPSPlatformSettings> m_PlatformSettings;

		#endregion


		#region Private Fields

		[NonSerialized]
		private int m_FPS;

		#endregion


		#region Private Properties

		private bool MonitorFPS => m_MonitorFPS;

		private List<FPSPlatformSettings> PlatformSettings => m_PlatformSettings = m_PlatformSettings ?? new List<FPSPlatformSettings>();

		#endregion


	}

	[Serializable]
	public class FPSPlatformSettings {


		#region Public Properties

		public RuntimePlatform Platform => m_Platform;

		public int TargetFrameRate => m_TargetFrameRate;

		public int VSyncCount => m_VSyncCount;

		#endregion


		#region Private Fields

		[SerializeField]
		private RuntimePlatform m_Platform;

		[SerializeField]
		private int m_TargetFrameRate = 60;

		[SerializeField]
		private int m_VSyncCount = 0;

		#endregion


	}

}