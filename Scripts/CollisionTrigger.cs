namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Triggers collision events when other <see cref="CollisionTrigger"/>s enter and exit this one and have
	/// <see cref="ThisTags"/> that match the <see cref="OtherTags"/>.
	/// </summary>
	public class CollisionTrigger : CollisionTriggerBase<CollisionTrigger, Collider, Collision> {


		#region Unity Methods

		private void OnTriggerStay(Collider other) {
			_OnTriggerStay(other);
		}

		private void OnCollisionStay(Collision collision) {
			_OnCollisionStay(collision);
		}

		#endregion


	}

}
