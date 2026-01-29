namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Intermediate non-generic class created to support a base property drawer for all subclasses.
	/// </summary>
	[Serializable]
	public class CompositeState : CompositeObject {


		#region Public Methods

		public virtual void Enter() { }

		public virtual void Exit() { }

		#endregion


		#region Unity Methods

		public virtual void Update() { }

		public virtual void FixedUpdate() { }

		public virtual void OnValidate() { }

		/// <summary>
		/// Invoked in addition to <see cref="OnValidate()"/> for cases where the validation
		/// requires access to the state machine in edit mode
		/// </summary>
		/// 
		/// <remarks>
		/// <paramref name="editModeMachine"/> is declared as <see cref="UnityEngine.Object"/>
		/// because it can be a <see cref="MonoBehaviour"/> or a <see cref="ScriptableObject"/>.
		/// </remarks>
		/// 
		/// <param name="editModeMachine">The state machine.</param>
		public virtual void OnValidate(UnityEngine.Object editModeMachine) { }

		public virtual void OnDestroy() { }

		#endregion





	}

}