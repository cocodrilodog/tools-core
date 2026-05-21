namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public static class AudioUtility {


		#region Public Static Methods

		// Source: https://answers.unity.com/questions/283192/how-to-convert-decibel-number-to-audio-source-volu.html
		public static float LinearToDecibel(float linear) {
			// Log10 is undefined for 0 or negative values.
			// Unity mixers usually clamp around -80 dB.
			if (linear <= 0.0f) {
				return -80.0f;
			}
			return 20.0f * Mathf.Log10(linear);
		}

		public static float DecibelToLinear(float dB) {
			return Mathf.Pow(10.0f, dB / 20.0f);
		}

		#endregion


	}

}