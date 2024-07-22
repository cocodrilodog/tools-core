namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Triggers collision events when other <see cref="CollisionTrigger2D"/>s enter and exit this one and have
	/// <see cref="ThisTags"/> that match the <see cref="OtherTags"/>.
	/// </summary>
	public class CollisionTrigger2D : CollisionTriggerBase<CollisionTrigger2D, Collider2D, Collision2D> {


		#region Unity Methods

		private void OnTriggerStay2D(Collider2D other) {
			_OnTriggerStay(other);
		}

		private void OnCollisionStay2D(Collision2D collision) {
			_OnCollisionStay(collision);
		}

		#endregion


	}

}
