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

}