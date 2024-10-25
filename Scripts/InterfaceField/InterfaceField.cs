namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A field that can be used to hold a reference to a UnityEngine.Object that implements
	/// the <typeparamref name="T"/> interface.
	/// </summary>
	/// <typeparam name="T">The type of the interface.</typeparam>
	[Serializable]
	public class InterfaceField<T> where T : class {


		#region Public Properties

		public T Value {
			get => m_Value as T;
			set => m_Value = value as UnityEngine.Object;
		}

		#endregion


		#region Private Fields

		[SerializeField]
		private UnityEngine.Object m_Value;

		#endregion


	}

}