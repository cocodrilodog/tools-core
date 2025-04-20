namespace CocodriloDog.Core {

	using System.Collections.Generic;
	using UnityEngine;

	public class StringOptions_Example : MonoBehaviour {


		#region Private Fields

		[Tooltip("A StringOptions asset.")]
		public StringOptions SomeStringOptions;

		[Tooltip("A string with values provided by the SomeStringOptions asset.")]
		[StringOptions(nameof(SomeStringOptions))] // From the SomeStringOptions field declared above
		public string StringOptions_FieldReferencedAsset;

		[Tooltip("A string with values provided by the StringOptions2_Example asset in a Resources folder.")]
		[StringOptions("StringOptions2_Example")] // From asset named StringOptions2_Example in a Resurces folder
		public string StringOptions_ResourcesAssets;

		[Tooltip("A string with values provided by the method GetStringOptions().")]
		[StringOptions(nameof(GetStringOptions))] // From the method below
		public string StringOptions_Method;

		private List<string> GetStringOptions() => new List<string> { "January", "February", "March" };

		[Tooltip("A string with a missing source.")]
		[StringOptions("SomeMissingSource")] // From missing source
		public string StringOptions_MissingSource;

		#endregion


	}

}