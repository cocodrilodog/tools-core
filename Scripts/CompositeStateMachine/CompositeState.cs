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

		public virtual void OnValidate(UnityEngine.Object editModeMachine) { }

		public virtual void OnDestroy() { }

		#endregion





	}

}