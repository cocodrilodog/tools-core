namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// This attribute draws a "Create" button on <c>ScriptableObject</c> fields
	/// to create a new asset of the type of the field.
	/// </summary>
	/// <remarks>
	/// The button is drawn only when there is no value assigned.
	/// </remarks>
	public class CreateAssetAttribute : PropertyAttribute { }

}