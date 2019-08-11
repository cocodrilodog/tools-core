﻿namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class Attributes_Example : MonoBehaviour {

		[MinMaxRange(0, 10)]
		[SerializeField]
		public MinMaxRange MinMaxRange;

		[Space]
		[HorizontalLine]
		public string SomeField;

	}
}