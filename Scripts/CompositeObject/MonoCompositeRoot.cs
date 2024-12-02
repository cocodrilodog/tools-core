namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A base MonoBehaviour class for root objects of composite structures that are made up from 
	/// concrete <see cref="CompositeObject"/>s.
	/// </summary>
	public class MonoCompositeRoot : MonoBehaviour, ICompositeRoot {


		#region Public Properties

		public string SelectedCompositePath {
			get => m_SelectedCompositePath;
			set => m_SelectedCompositePath = value;
		}

		#endregion


		#region Private Fields

		[NonSerialized]
		public string m_SelectedCompositePath;

		#endregion


	}

}