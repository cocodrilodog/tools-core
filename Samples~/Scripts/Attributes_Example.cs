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

		// TODO: Add support for lists
		[CreateAsset]
		public List<StringOptions> CreateManyAssets;

		public StringOptions SomeStringOptions;

		[StringOptions(nameof(SomeStringOptions))] // From the SomeStringOptions field
		public string StringOptions_FieldReferencedAsset;

		[StringOptions("StringOptions2_Example")] // From asset named StringOptions2_Example in a Resurces folder
		public string StringOptions_ResourcesAssets;

		[StringOptions(nameof(GetStringOptions))] // From the method below
		public string StringOptions_Method;

		private List<string> GetStringOptions() => new List<string> { "January", "February", "March" };

		[StringOptions("SomeMissingSource")] // From missing source
		public string StringOptions_MissingSource;

		[StringOptions(nameof(SomeStringOptions))] // From the SomeStringOptions field
		public List<string> StringOptions_ListAsset;

		[StringOptions(nameof(GetStringOptions))] // From the GetStringOptions method
		public List<string> StringOptions_ListMethod;

		[Range(-3, 3)]
		public int HelpValue;

		[Help(nameof(StringsHelp))]
		public string StringWithHelp = "A string with help";

		// TODO: Show warning when using in a normal list
		[Help("StringsHelp")]
		public ListWrapper<string> StringsWithHelp;

		private int StringsHelp(ref string message) {
			message = "Showing some help!";
			return HelpValue;
		}

		public bool ShowHiddenString;

		[Hide(nameof(HideString), 1)]
		public string HiddenString = "Hidden String";

		// TODO: Show warning when using in a normal list
		[Hide(nameof(HideString), 1)]
		public ListWrapper<string> HiddenStrings = new ListWrapper<string>();

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

		[StringOptions(nameof(SomeNestedStringOptions))] // From the SomeNestedStringOptions field
		//[StringOptions("SomeStringOptions")] // From the SomeStringOptions field on he root serialized object
		public string StringOptions_FieldReferencedAsset;

		[StringOptions(nameof(GetNestedStringOptions))] // From the method below
		//[StringOptions("GetStringOptions")] // From the method from the root serialized object
		public string StringOptions_Method;

		private List<string> GetNestedStringOptions() => new List<string> { "Monday", "Tuesday", "Wednesday" };

		[StringOptions("SomeMissingSource")] // From missing source
		public string StringOptions_MissingSource;

		[StringOptions(nameof(SomeNestedStringOptions))] // From the SomeNestedStringOptions field
		//[StringOptions("SomeStringOptions")] // From the SomeStringOptions field on he root serialized object
		public List<string> StringOptions_ListAsset;

		[StringOptions(nameof(GetNestedStringOptions))] // From the GetNestedStringOptions method
		//[StringOptions("GetStringOptions")] // From the method from the root serialized object
		public List<string> StringOptions_ListMethod;

		public bool ShowHiddenObject;

		[Hide(nameof(HideObject), 1)]
		public UnityEngine.Object HiddenObject;

		[Hide(nameof(HideObject), 1)]
		public ListWrapper<UnityEngine.Object> HiddenObjects;

		private bool HideObject() => !ShowHiddenObject;

		[Range(-3, 3)]
		public int HelpValue;

		[Help(nameof(IntHelp))]
		public int IntWithHelp;

		[Help(nameof(IntHelp))]
		public ListWrapper<int> IntsWithHelp;

		private int IntHelp(ref string message) {
			message = "Showing some integral help!";
			return HelpValue;
		}

	}

}