namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A base class for root objects of composite structures that are made up from concrete
	/// <see cref="CompositeObject"/>s.
	/// </summary>
	public class CompositeRoot : MonoBehaviour {

#if UNITY_EDITOR

		#region Private Fields

		[HideInInspector]
		[NonSerialized]
		public string SelectedCompositePath;

		#endregion

#endif

	}

}