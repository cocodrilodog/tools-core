namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Interface for root objects of composite structures that are made up from concrete
	/// <see cref="CompositeObject"/>s.
	/// </summary>
	public interface ICompositeRoot {

		string SelectedCompositePath { get; set; }

	}

}