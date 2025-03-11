namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(menuName = "Cocodrilo Dog/Core/Scriptable Reference")]
	public class ScriptableReference : ScriptableValue<UnityEngine.Object> {


		#region Public Properties

		/// <summary>
		/// The value that is stored/referenced by this asset.
		/// </summary>
		public override UnityEngine.Object Value {
			get => base.Value;
			set {
				if (value != base.Value && base.Value != null) {
					FinalizeTime?.Invoke(Value);
				}
				base.Value = value;
				if (Value != null) {
					m_InitializeTime?.Invoke(Value);
				}
			}
		}

		#endregion


		#region Public Delegates

		public delegate void ReferenceChange(UnityEngine.Object unityObject);

		#endregion


		#region Public Events

		/// <summary>
		/// Raised immediatly when adding a handler, if the reference value is non-null, and every time a non-null
		/// reference value is set.
		/// </summary>
		public event ReferenceChange InitializeTime {
			add {
				var shouldInvokeNow = false;
				lock (this) {
					shouldInvokeNow = Value != null;
					m_InitializeTime += value;
				}
				if (shouldInvokeNow) {
					m_InitializeTime?.Invoke(Value);
				}
			}
			remove {
				lock (this) {
					m_InitializeTime -= value;
				}
			}
		}

		/// <summary>
		/// Raised when the <c>Value</c> is about to change and the current value (soon to become old) 
		/// is not null.
		/// </summary>
		/// 
		/// <remarks>
		/// Use this to clean old references that won't be used anymore. E.G. removing event handlers,
		/// disposing objects, etc.
		/// </remarks>
		public event ReferenceChange FinalizeTime;

		#endregion


		#region Private Fields

		[NonSerialized]
		private ReferenceChange m_InitializeTime;

		#endregion


	}

}
