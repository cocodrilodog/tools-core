namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public static class AudioUtility {


		#region Public Static Methods

		// Source: https://answers.unity.com/questions/283192/how-to-convert-decibel-number-to-audio-source-volu.html
		public static float LinearToDecibel(float linear) {
			float dB;
			// TODO: Check if this must be "if (linear > 0)" instead
			if (linear != 0) {
				dB = 20.0f * Mathf.Log10(linear);
			} else {
				dB = -144.0f;
			}
			return dB;
		}

		public static float DecibelToLinear(float dB) => Mathf.Pow(10.0f, dB / 20.0f);

		#endregion


	}

}