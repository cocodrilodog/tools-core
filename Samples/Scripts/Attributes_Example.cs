namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class Attributes_Example : MonoBehaviour {

		[MinMaxRange(0, 10)]
		[SerializeField]
		public FloatRange MinMaxRange;

		[HorizontalLine]
		[Space]
		public string SomeField;

	}
}