namespace CocodriloDog.Core.Examples {
	using System;
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
		public List<string> StringOptions_ListAsset;

		[StringOptions("GetStringOptions")] // From the GetStringOptions method
		public List<string> StringOptions_ListMethod;

		[Range(-3, 3)]
		public int HelpValue;

		[Help("StringsHelp")]
		public string StringWithHelp = "A string with help";

		// TODO: This is not looking good and may benefit from the ListWrapper<T> mentioned
		// below so that the help is for the list and not for each item.
		[Help("StringsHelp")]
		public List<string> StringsWithHelp;

		private int StringsHelp(ref string message) {
			message = "Showing some help!";
			return HelpValue;
		}

		public bool ShowHiddenString;

		[Hide("HideString", 1)]
		public string HiddenString = "Hidden String";

		// TODO: List property drawers are not allowed. It may be needed to create
		// ListWrapper<T> as a wrapper for the list. Similar to CompositeList
		//[Hide("HideString", 1)]
		//public List<string> HiddenStrings = new List<string>();

		private bool HideString() => !ShowHiddenString;

		[Button(16)]
		public void ButtonMethod() {
			Debug.Log("Button Method Invoked!");
		}

		public SomeObject SomeObject;

		[Space]

		[UnityEventGroup("EventGroup")]
		public UnityEvent<Vector3> Event1;

		[UnityEventGroup("EventGroup")]
		public UnityEvent Event2;

		[UnityEventGroup("LonelyEvent")]
		public UnityEvent Event3;


	}

	// Test the attributes in a nested system object
	[Serializable]
	public class SomeObject {

		public StringOptions SomeNestedStringOptions;

		[StringOptions("SomeNestedStringOptions")] // From the SomeNestedStringOptions field
		//[StringOptions("SomeStringOptions")] // From the SomeStringOptions field on he root serialized object
		public string StringOptions_FieldReferencedAsset;

		[StringOptions("GetNestedStringOptions")] // From the method below
		//[StringOptions("GetStringOptions")] // From the method from the root serialized object
		public string StringOptions_Method;

		private List<string> GetNestedStringOptions() => new List<string> { "Monday", "Tuesday", "Wednesday" };

		[StringOptions("SomeMissingSource")] // From missing source
		public string StringOptions_MissingSource;

		[StringOptions("SomeNestedStringOptions")] // From the SomeNestedStringOptions field
		//[StringOptions("SomeStringOptions")] // From the SomeStringOptions field on he root serialized object
		public List<string> StringOptions_ListAsset;

		[StringOptions("GetNestedStringOptions")] // From the GetNestedStringOptions method
		//[StringOptions("GetStringOptions")] // From the method from the root serialized object
		public List<string> StringOptions_ListMethod;

		public bool ShowHiddenObject;

		[Hide("HideObject", 1)]
		public UnityEngine.Object HiddenObject;

		private bool HideObject() => !ShowHiddenObject;

		[Range(-3, 3)]
		public int HelpValue;

		[Help("IntHelp")]
		public int IntWithHelp;

		[Help("IntHelp")]
		public List<int> IntsWithHelp;

		private int IntHelp(ref string message) {
			message = "Showing some integral help!";
			return HelpValue;
		}

	}

}