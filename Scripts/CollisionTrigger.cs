namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Triggers collision events when other <see cref="CollisionTrigger"/>s enter this one and have
	/// <see cref="SelfTags"/> that match the <see cref="OtherTags"/>. Optionally, <see cref="OtherParents"/>
	/// can be provided to limit the enter/exit events to one per parent, when the parent has multiple colliders.
	/// </summary>
	public class CollisionTrigger : MonoBehaviour {


		#region Public Properties

		/// <summary>
		/// A set of tags for this trigger to be identified by other triggers.
		/// </summary>
		public List<string> SelfTags {
			get => m_SelfTags;
			set => m_SelfTags = value;
		}

		/// <summary>
		/// Tags of other triggers that when collide with this one, they will make this 
		/// trigger to raise collision events.
		/// </summary>
		public List<string> OtherTags {
			get => m_OtherTags;
			set => m_OtherTags = value;
		}

		/// <summary>
		/// Gets the <see cref="CollisionTrigger"/>s that are currently collisioning with this one.
		/// </summary>
		/// <returns>A read-only collection with the triggers.</returns>
		public ReadOnlyCollection<CollisionTrigger> OtherCollisioning =>
			m_OtherCollisioning_RO = m_OtherCollisioning_RO ?? new ReadOnlyCollection<CollisionTrigger>(m_OtherCollisioning);

		#endregion


		#region Public Methods

		/// <summary>
		/// Returns <c>true</c> if the provided <paramref name="otherTrigger"/> is collisioning with this 
		/// game object, otherwise <c>false</c>.
		/// </summary>
		/// <param name="otherTrigger">Other <see cref="CollisionTrigger"/></param>
		/// <returns></returns>
		public bool IsOtherCollisioning(CollisionTrigger otherTrigger) => m_OtherCollisioningCount.ContainsKey(otherTrigger);

		#endregion


		#region Public Events

		public event Action<CollisionTrigger> OnTriggerEnterEv;

		public event Action<CollisionTrigger> OnTriggerExitEv;

		public event Action<Collision> OnCollisionEnterEv;

		public event Action<Collision> OnCollisionExitEv;

		#endregion


		#region Unity Methods

		private void OnTriggerEnter(Collider other) {
			var otherTrigger = other.GetComponentInParent<CollisionTrigger>();
			if (otherTrigger != null && DoOtherTagsMatch(otherTrigger) && EnterTrigger(otherTrigger)) {
				OnTriggerEnterEv?.Invoke(otherTrigger);
				m_OnTriggerEnter.Invoke(otherTrigger);
			}
		}

		private void OnTriggerExit(Collider other) {
			var otherTrigger = other.GetComponentInParent<CollisionTrigger>();
			if (otherTrigger != null && DoOtherTagsMatch(otherTrigger) && ExitTrigger(otherTrigger)) {
				OnTriggerExitEv?.Invoke(otherTrigger);
				m_OnTriggerExit.Invoke(otherTrigger);
			}
		}

		public void OnCollisionEnter(Collision collision) {
			var otherTrigger = collision.collider.GetComponentInParent<CollisionTrigger>();
			if (otherTrigger != null && DoOtherTagsMatch(otherTrigger) && EnterTrigger(otherTrigger)) {
				OnCollisionEnterEv?.Invoke(collision);
				m_OnCollisionEnter.Invoke(collision);
			}
		}

		private void OnCollisionExit(Collision collision) {
			var otherTrigger = collision.collider.GetComponentInParent<CollisionTrigger>();
			if (otherTrigger != null && DoOtherTagsMatch(otherTrigger) && ExitTrigger(otherTrigger)) {
				OnCollisionExitEv?.Invoke(collision);
				m_OnCollisionExit.Invoke(collision);
			}
		}

		private void OnDestroy() {
			OnTriggerEnterEv = null;
			OnTriggerExitEv = null;
		}

		#endregion


		#region Private Fields - Serialized

		[Tooltip("The tag options that will be used in Self Tag and Other Tags.")]
		[SerializeField]
		private StringOptions m_TagOptions;

		[Tooltip("A set of tags for this trigger to be identified by other triggers.")]
		[StringOptions("m_TagOptions")]
		[SerializeField]
		private List<string> m_SelfTags;

		[Tooltip(
			"Tags of other triggers that when collide with this one, " +
			"they will make this trigger to raise collision events."
		)]
		[StringOptions("m_TagOptions")]
		[SerializeField]
		private List<string> m_OtherTags;

		[Space]

		[Tooltip("Raised when another collision trigger with SelfTags that match the OtherTags of this one enters this one.")]
		[UnityEventGroup("TriggerEvents")]
		[SerializeField]
		private UnityEvent<CollisionTrigger> m_OnTriggerEnter;

		[Tooltip("Raised when another collision trigger with SelfTags that match the OtherTags of this one exits this one.")]
		[UnityEventGroup("TriggerEvents")]
		[SerializeField]
		private UnityEvent<CollisionTrigger> m_OnTriggerExit;

		[Tooltip("Raised when another collision trigger with SelfTags that match the OtherTags of this one enters this one.")]
		[UnityEventGroup("CollisionEvents")]
		[SerializeField]
		private UnityEvent<Collision> m_OnCollisionEnter;

		[Tooltip("Raised when another collision trigger with SelfTags that match the OtherTags of this one exits this one.")]
		[UnityEventGroup("CollisionEvents")]
		[SerializeField]
		private UnityEvent<Collision> m_OnCollisionExit;

		#endregion


		#region Private Fields - Non Serialized

		[NonSerialized]
		private Dictionary<CollisionTrigger, int> m_OtherCollisioningCount = new Dictionary<CollisionTrigger, int>();

		[NonSerialized]
		private List<CollisionTrigger> m_OtherCollisioning = new List<CollisionTrigger>();

		[NonSerialized]
		private ReadOnlyCollection<CollisionTrigger> m_OtherCollisioning_RO;

		#endregion


		#region Private Methods

		private bool DoOtherTagsMatch(CollisionTrigger otherTrigger) {
			foreach (var otherTag in otherTrigger.m_SelfTags) {
				if (m_OtherTags.Contains(otherTag)) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Registers that the <paramref name="otherTrigger"/> enters this one.
		/// </summary>
		/// <remarks>
		/// This handles the case where a trigger may have multiple colliders so that the enter events
		/// are only raised when the first collider enters.
		/// </remarks>
		/// <param name="otherTrigger">The other trigger</param>
		/// <returns><c>true</c> if the <paramref name="otherTrigger"/> is entering with its first collider.</returns>
		private bool EnterTrigger(CollisionTrigger otherTrigger) {
			if (!m_OtherCollisioningCount.ContainsKey(otherTrigger)) {
				m_OtherCollisioningCount[otherTrigger] = 1;
				m_OtherCollisioning.Add(otherTrigger);
				return true;
			} else {
				m_OtherCollisioningCount[otherTrigger]++;
				return false;
			}
		}

		/// <summary>
		/// Registers that the <paramref name="otherTrigger"/> exits this one.
		/// </summary>
		/// <remarks>
		/// This handles the case where a trigger may have multiple colliders so that the exit events
		/// are only raised when the last collider exits.
		/// </remarks>
		/// <param name="otherTrigger">The other trigger</param>
		/// <returns><c>true</c> if the <paramref name="otherTrigger"/> is exiting with its last collider.</returns>
		private bool ExitTrigger(CollisionTrigger otherTrigger) {
			if (m_OtherCollisioningCount.ContainsKey(otherTrigger)) {
				m_OtherCollisioningCount[otherTrigger]--;
				if (m_OtherCollisioningCount[otherTrigger] == 0) {
					m_OtherCollisioningCount.Remove(otherTrigger);
					m_OtherCollisioning.Remove(otherTrigger);
					return true;
				}
			}
			return false;
		}

		#endregion


	}

}
