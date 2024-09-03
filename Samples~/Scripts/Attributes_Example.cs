namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	[AddComponentMenu("")]
	public class Attributes_Example : MonoBehaviour {

		[MinMaxRange(0, 10)]
		public FloatRange MinMaxRange;


		[MinMaxRange(0, 10)]
		public List<FloatRange> MinMaxRanges;

		[HorizontalLine]
		[Space]

		[CreateAsset]
		public StringOptions CreateAsset;

		public StringOptions SomeStringOptions;

		[StringOptions("SomeStringOptions")] // From the SomeStringOptions field
		public string StringOptions_FieldReferencedAsset;

		[StringOptions("StringOptions2_Example")] // From asset named StringOptions2_Example in a Resurces folder
		public string StringOptions_ResourcesAssets;

		[StringOptions("GetStringOptions")] // From the method below
		public string StringOptions_Method;

		private List<string> GetStringOptions() => new List<string> { "January", "February", "March" };

		[StringOptions("SomeMissingSource")] // From missing source
		public string StringOptions_MissingSource;

		[StringOptions("SomeStringOptions")] // From the SomeStringOptions field
		public List<string> StringOptions_List;

		[Range(-3, 3)]
		public int m_HelpValue;

		[Help("HelpHelp")]
		[SerializeField]
		public string Help;

		[Help("HelpHelp")]
		[SerializeField]
		public List<string> Helps;

		private int HelpHelp(ref string message) {
			message = "Showing some help!";
			return m_HelpValue;
		}

		public bool m_ShowHiddenString;

		[Hide("IsHiddenStringHidden", 1)]
		public string m_HiddenString = "Hidden String";

		private bool IsHiddenStringHidden() => !m_ShowHiddenString;

		[Button(14)]
		public void ButtonMethod() {
			Debug.Log("Button Method Invoked!");
		}

		[Space]

		[UnityEventGroup("EventGroup")]
		public UnityEvent<Vector3> Event1;

		[UnityEventGroup("EventGroup")]
		public UnityEvent Event2;

		[UnityEventGroup("LonelyEvent")]
		public UnityEvent Event3;


	}

}