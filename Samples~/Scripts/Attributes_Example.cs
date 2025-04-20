namespace CocodriloDog.Core.Examples {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	[AddComponentMenu("")]
	public class Attributes_Example : MonoBehaviour {

		[Tooltip("MinMaxRange")]
		[MinMaxRange(0, 10)]
		public FloatRange MinMaxRange;

		[Tooltip("MinMaxRanges")]
		[MinMaxRange(0, 10)]
		public List<FloatRange> MinMaxRanges;

		[HorizontalLine]
		[Space]

		[Tooltip("CreateAsset")]
		[CreateAsset]
		public StringOptions CreateAsset;

		[Tooltip("CreateAssets")]
		[CreateAsset]
		public List<StringOptions> CreateManyAssets;

		[Tooltip("SomeStringOptions")]
		public StringOptions SomeStringOptions;

		[Tooltip("StringOptions_FieldReferencedAsset")]
		[StringOptions(nameof(SomeStringOptions))] // From the SomeStringOptions field
		public string StringOptions_FieldReferencedAsset;

		[Tooltip("StringOptions_ResourcesAssets")]
		[StringOptions("StringOptions2_Example")] // From asset named StringOptions2_Example in a Resurces folder
		public string StringOptions_ResourcesAssets;

		[Tooltip("StringOptions_Method")]
		[StringOptions(nameof(GetStringOptions))] // From the method below
		public string StringOptions_Method;

		private List<string> GetStringOptions() => new List<string> { "January", "February", "March" };

		[Tooltip("StringOptions_MissingSource")]
		[StringOptions("SomeMissingSource")] // From missing source
		public string StringOptions_MissingSource;

		[Tooltip("StringOptions_ListAsset")]
		[StringOptions(nameof(SomeStringOptions))] // From the SomeStringOptions field
		public List<string> StringOptions_ListAsset;

		[Tooltip("StringOptions_ListMethod")]
		[StringOptions(nameof(GetStringOptions))] // From the GetStringOptions method
		public List<string> StringOptions_ListMethod;

		[Range(-3, 3)]
		public int HelpValue;

		[Tooltip("StringWithHelp")]
		[Help(nameof(StringsHelp))]
		public string StringWithHelp = "A string with help";

		// TODO: Show warning when using in a normal list
		[Tooltip("StringsWithHelp")]
		[Help("StringsHelp")]
		public ListWrapper<string> StringsWithHelp;

		private int StringsHelp(ref string message) {
			message = "Showing some help!";
			return HelpValue;
		}

		public bool ShowHiddenString;

		[Tooltip("HiddenString")]
		[Hide(nameof(HideString), 1)]
		public string HiddenString = "Hidden String";

		// TODO: Show warning when using in a normal list
		[Tooltip("HiddenStrings")]
		[Hide(nameof(HideString), 1)]
		public ListWrapper<string> HiddenStrings = new ListWrapper<string>();

		private bool HideString() => !ShowHiddenString;

		[Button(18, disableInEditMode: true)]
		public void ButtonMethod1() {
			Debug.Log("Button Method 1 Invoked!");
		}

		[Button(18, horizontalizeSameIndex:true, disableInPlayMode: true)]
		public void ButtonMethod2() {
			Debug.Log("Button Method 2 Invoked!");
		}

		[Tooltip("ReadOnlyString")]
		[ReadOnly]
		public string ReadOnlyString = "This is a read-only string.";

		[Space]

		[Tooltip("Event1")]
		[UnityEventGroup("EventGroup")]
		public UnityEvent<Vector3> Event1;

		[Tooltip("Event2")]
		[UnityEventGroup("EventGroup")]
		public UnityEvent Event2;

		[Tooltip("Event3")]
		[UnityEventGroup("LonelyEvent")]
		public UnityEvent Event3;

		[Tooltip("SomeObject")]
		public SomeObject SomeObject;

		[Tooltip("SomeCDObject")]
		public SomeCDObject SomeCDObject;

	}

	// Test the attributes in a nested system object
	[Serializable]
	public class SomeObject {

		[Tooltip("SomeNestedStringOptions")]
		public StringOptions SomeNestedStringOptions;

		[Tooltip("StringOptions_FieldReferencedAsset")]
		[StringOptions(nameof(SomeNestedStringOptions))] // From the SomeNestedStringOptions field
		//[StringOptions("SomeStringOptions")] // From the SomeStringOptions field on he root serialized object
		public string StringOptions_FieldReferencedAsset;

		[Tooltip("StringOptions_ResourcesAssets")]
		[StringOptions("StringOptions2_Example")] // From asset named StringOptions2_Example in a Resurces folder
		public string StringOptions_ResourcesAssets;

		[Tooltip("StringOptions_Method")]
		[StringOptions(nameof(GetNestedStringOptions))] // From the method below
		//[StringOptions("GetStringOptions")] // From the method from the root serialized object
		public string StringOptions_Method;

		private List<string> GetNestedStringOptions() => new List<string> { "Monday", "Tuesday", "Wednesday" };

		[Tooltip("StringOptions_MissingSource")]
		[StringOptions("SomeMissingSource")] // From missing source
		public string StringOptions_MissingSource;

		// For the ButtonAttribute to work in System.Object, the object must be drawn by CDObjectPropertyDrawer
		// of a child class. In this case, I implemented SomeObjectPropertyDrawer.
		// Alternatively, the object may inherit from CDObject, which supports the ButtonAttribute by default.
		[Button(3, horizontalizeSameIndex:true, disableInEditMode: true)]
		public void SystemObjectButtonTest1() {
			Debug.Log("System Object Button test 1");
		}

		[Button(3, disableInPlayMode: true)]
		public void SystemObjectButtonTest2() {
			Debug.Log("System Object Button test 2");
		}

		[Tooltip("StringOptions_ListAsset")]
		[StringOptions(nameof(SomeNestedStringOptions))] // From the SomeNestedStringOptions field
		//[StringOptions("SomeStringOptions")] // From the SomeStringOptions field on he root serialized object
		public List<string> StringOptions_ListAsset;

		[Tooltip("StringOptions_ListMethod")]
		[StringOptions(nameof(GetNestedStringOptions))] // From the GetNestedStringOptions method
		//[StringOptions("GetStringOptions")] // From the method from the root serialized object
		public List<string> StringOptions_ListMethod;

		public bool ShowHiddenObject;

		[Tooltip("HiddenObject")]
		[Hide(nameof(HideObject), 1)]
		public UnityEngine.Object HiddenObject;

		[Tooltip("HiddenObjects")]
		[Hide(nameof(HideObject), 1)]
		public ListWrapper<UnityEngine.Object> HiddenObjects;

		private bool HideObject() => !ShowHiddenObject;

		[Range(-3, 3)]
		public int HelpValue;

		[Tooltip("IntWithHelp")]
		[Help(nameof(IntHelp))]
		public int IntWithHelp;

		[Tooltip("IntsWithHelp")]
		[Help(nameof(IntHelp))]
		public ListWrapper<int> IntsWithHelp;

		private int IntHelp(ref string message) {
			message = "Showing some integral help!";
			return HelpValue;
		}

		[Tooltip("ReadOnlyString")]
		[ReadOnly]
		public string ReadOnlyString = "This is a read-only string.";

		[Tooltip("ObjEvent1")]
		[UnityEventGroup("ObjEventGroup")]
		public UnityEvent<Vector3> ObjEvent1;

		[Tooltip("ObjEvent2")]
		[UnityEventGroup("ObjEventGroup")]
		public UnityEvent ObjEvent2;

		[Tooltip("ObjEvent3")]
		[UnityEventGroup("ObjLonelyEvent")]
		public UnityEvent ObjEvent3;

	}

	// Test the CDObject which uses the CDObjectPropertyDrawer by hence supports the ButtonAttribute.
	[Serializable]
	public class SomeCDObject : CDObject {

		[Tooltip("SomeNestedStringOptions")]
		public StringOptions SomeNestedStringOptions;

		[Tooltip("StringOptions_FieldReferencedAsset")]
		[StringOptions(nameof(SomeNestedStringOptions))] // From the SomeNestedStringOptions field
		//[StringOptions("SomeStringOptions")] // From the SomeStringOptions field on he root serialized object
		public string StringOptions_FieldReferencedAsset;

		[Tooltip("StringOptions_ResourcesAssets")]
		[StringOptions("StringOptions2_Example")] // From asset named StringOptions2_Example in a Resurces folder
		public string StringOptions_ResourcesAssets;

		[Tooltip("StringOptions_Method")]
		[StringOptions(nameof(GetNestedStringOptions))] // From the method below
		//[StringOptions("GetStringOptions")] // From the method from the root serialized object
		public string StringOptions_Method;

		private List<string> GetNestedStringOptions() => new List<string> { "Monday", "Tuesday", "Wednesday" };

		[Tooltip("StringOptions_MissingSource")]
		[StringOptions("SomeMissingSource")] // From missing source
		public string StringOptions_MissingSource;

		// This class inherits from CDObject, which supports the ButtonAttribute by default.
		[Button(3, horizontalizeSameIndex:true, disableInEditMode:true)]
		public void CDObjectButtonTest1() {
			Debug.Log("CDObject Button test 1");
		}

		[Button(3, disableInPlayMode:true)]
		public void CDObjectButtonTest2() {
			Debug.Log("CDObject Button test 2");
		}

		[Tooltip("StringOptions_ListAsset")]
		[StringOptions(nameof(SomeNestedStringOptions))] // From the SomeNestedStringOptions field
														 //[StringOptions("SomeStringOptions")] // From the SomeStringOptions field on he root serialized object
		public List<string> StringOptions_ListAsset;

		[Tooltip("StringOptions_ListMethod")]
		[StringOptions(nameof(GetNestedStringOptions))] // From the GetNestedStringOptions method
														//[StringOptions("GetStringOptions")] // From the method from the root serialized object
		public List<string> StringOptions_ListMethod;

		public bool ShowHiddenObject;

		[Tooltip("HiddenObject")]
		[Hide(nameof(HideObject), 1)]
		public UnityEngine.Object HiddenObject;

		[Tooltip("HiddenObjects")]
		[Hide(nameof(HideObject), 1)]
		public ListWrapper<UnityEngine.Object> HiddenObjects;

		private bool HideObject() => !ShowHiddenObject;

		[Range(-3, 3)]
		public int HelpValue;

		[Tooltip("IntWithHelp")]
		[Help(nameof(IntHelp))]
		public int IntWithHelp;

		[Tooltip("IntsWithHelp")]
		[Help(nameof(IntHelp))]
		public ListWrapper<int> IntsWithHelp;

		private int IntHelp(ref string message) {
			message = "Showing some integral help!";
			return HelpValue;
		}

		[Tooltip("ReadOnlyString")]
		[ReadOnly]
		public string ReadOnlyString = "This is a read-only string.";

		[Tooltip("ObjEvent1")]
		[UnityEventGroup("ObjEventGroup")]
		public UnityEvent<Vector3> ObjEvent1;

		[Tooltip("ObjEvent2")]
		[UnityEventGroup("ObjEventGroup")]
		public UnityEvent ObjEvent2;

		[Tooltip("ObjEvent3")]
		[UnityEventGroup("ObjLonelyEvent")]
		public UnityEvent ObjEvent3;

	}

}