namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Triggers collision events when other <see cref="CollisionTrigger2D"/>s enter and exit this one and have
	/// <see cref="SelfTags"/> that match the <see cref="OtherTags"/>.
	/// </summary>
	public class CollisionTrigger2D : CollisionTriggerBase<CollisionTrigger2D, Collision2D> {


		#region Unity Methods

		private void OnTriggerEnter2D(Collider2D other) {
			var otherTrigger = other.GetComponentInParent<CollisionTrigger2D>();
			if (otherTrigger != null && DoOtherTagsMatch(otherTrigger) && EnterTrigger(otherTrigger)) {
				RaiseTriggerEnter(otherTrigger);
				m_OnTriggerEnter.Invoke(otherTrigger);
			}
		}

		private void OnTriggerExit2D(Collider2D other) {
			var otherTrigger = other.GetComponentInParent<CollisionTrigger2D>();
			if (otherTrigger != null && DoOtherTagsMatch(otherTrigger) && ExitTrigger(otherTrigger)) {
				RaiseTriggerExit(otherTrigger);
				m_OnTriggerExit.Invoke(otherTrigger);
			}
		}

		private void OnCollisionEnter2D(Collision2D collision) {
			var otherTrigger = collision.collider.GetComponentInParent<CollisionTrigger2D>();
			if (otherTrigger != null && DoOtherTagsMatch(otherTrigger) && EnterTrigger(otherTrigger)) {
				RaiseCollisionEnter(collision);
				m_OnCollisionEnter.Invoke(collision);
			}
		}

		private void OnCollisionExit2D(Collision2D collision) {
			var otherTrigger = collision.collider.GetComponentInParent<CollisionTrigger2D>();
			if (otherTrigger != null && DoOtherTagsMatch(otherTrigger) && ExitTrigger(otherTrigger)) {
				RaiseCollisionExit(collision);
				m_OnCollisionExit.Invoke(collision);
			}
		}

		#endregion


	}

}
