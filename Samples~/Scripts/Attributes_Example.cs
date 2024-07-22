namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	[AddComponentMenu("")]
	public class Attributes_Example : MonoBehaviour {

		[MinMaxRange(0, 10)]
		[SerializeField]
		public FloatRange MinMaxRange;

		[HorizontalLine]
		[Space]

		public StringOptions SomeStringOptions;

		[StringOptions("SomeStringOptions")] // From SomeStringOptions
		public string ChooseAString;

		[StringOptions("StringOptions2_Example")] // From asset named StringOptions2_Example in a Resurces folder
		public string ChooseAString2;

		[StringOptions("GetStringOptions")] // From the method below
		public string ChooseAString3;

		[StringOptions("SomeMissingSource")] // From missing source
		public string ChooseAString4;

		private List<string> GetStringOptions() => new List<string> { "January", "February", "March" };

		[Help("HelpHelp")]
		[SerializeField]
		public string Help;

		[Space]

		[UnityEventGroup("EventGroup")]
		public UnityEvent<Vector3> Event1;

		[UnityEventGroup("EventGroup")]
		public UnityEvent Event2;

		[UnityEventGroup("LonelyEvent")]
		public UnityEvent Event3;

		private int HelpHelp(ref string message) {
			message = "Showing some help!";
			return -1;
		}

	}

}