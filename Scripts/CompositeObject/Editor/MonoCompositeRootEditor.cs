namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Default editor for <see cref="MonoCompositeRoot"/> classes
	/// </summary>
	[CustomEditor(typeof(MonoCompositeRoot), true)]
	public class MonoCompositeRootEditor : CompositeRootEditor { }

}