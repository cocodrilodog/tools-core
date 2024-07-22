namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Triggers collision events when other <see cref="T_CollisionTrigger"/>s enter and exit this one and have
	/// <see cref="ThisTags"/> that match the <see cref="OtherTags"/>.
	/// </summary>
	public abstract class CollisionTriggerBase<T_CollisionTrigger, T_Collider, T_Collision> : MonoBehaviour
		where T_CollisionTrigger : CollisionTriggerBase<T_CollisionTrigger, T_Collider, T_Collision>
		where T_Collider : Component
		where T_Collision : class {


		#region Public Properties

		/// <summary>
		/// A set of tags for this trigger to be identified by other triggers.
		/// </summary>
		public List<string> ThisTags {
			get => m_ThisTags;
			set => m_ThisTags = value;
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
		/// Gets the collision triggers that are currently collisioning with this one.
		/// </summary>
		/// <returns>The list</returns>
		public List<T_CollisionTrigger> OtherStaying => m_StayingCollisionTriggers.Keys.ToList();

		#endregion


		#region Public Methods

		/// <summary>
		/// Returns <c>true</c> if the provided <paramref name="otherTrigger"/> is collisioning with this 
		/// game object, otherwise <c>false</c>.
		/// </summary>
		/// <param name="otherTrigger">Other collision trigger</param>
		/// <returns></returns>
		public bool IsOtherStaying(T_CollisionTrigger otherTrigger) => m_StayingCollisionTriggers.ContainsKey(otherTrigger);

		/// <summary>
		/// Returns <c>true</c> if there is any collision trigger with self tag <paramref name="otherTag"/>
		/// collisioning with this trigger.
		/// </summary>
		/// <param name="otherTag"></param>
		/// <returns></returns>
		public bool IsOtherStaying(string otherTag) {
			var otherCollisioning = m_StayingCollisionTriggers.Keys.FirstOrDefault(t => t.ThisTags.Contains(otherTag));
			return otherCollisioning != null;
		}

		#endregion


		#region Public Events

		public event Action<T_CollisionTrigger> OnTriggerEnterEv;

		public event Action<T_CollisionTrigger> OnTriggerExitEv;

		public event Action<T_Collision> OnCollisionEnterEv;

		public event Action<T_Collision> OnCollisionExitEv;

		#endregion


		#region Unity Methods

		private void FixedUpdate() {

			// Add the latest staying colliders
			foreach (var collider in m_TempStayingColliders) {
				if (m_StayingColliders.Add(collider)) {
					Debug.Log($"+ {collider.name}");
					var collisionTrigger = collider.GetComponentInParent<T_CollisionTrigger>(true);
					if (collisionTrigger != null && EnterCollisionTrigger(collisionTrigger, collider)) {
						if (m_StayingCollisionWraps.TryGetValue(collider, out var collisionWrap)) {
							RaiseCollisionEnter(collisionWrap.Collision);
						} else {
							RaiseTriggerEnter(collisionTrigger);
						}
					}
					// TODO: This would be the place for granular OnEnter
				}
			}

			// Mark the ones that are not staying anymore
			List<T_Collider> collidersToRemove = null;
			foreach (var collider in m_StayingColliders) {
				if (!m_TempStayingColliders.Contains(collider)) {
					collidersToRemove = collidersToRemove ?? new List<T_Collider>();
					collidersToRemove.Add(collider);
				}
			}

			// Remove them, if any
			if (collidersToRemove != null) {
				foreach (var collider in collidersToRemove) {
					if (m_StayingColliders.Remove(collider)) {
						Debug.Log($"- {collider.name}");
						// TODO: This would be the place for granular OnExit
						var collisionTrigger = collider.GetComponentInParent<T_CollisionTrigger>(true);
						if (collisionTrigger != null && ExitCollisionTrigger(collisionTrigger, collider) &&
							// This prevents the on exit to be caused by a collider that was deactivated, 
							// but we are still keeping the colliders count clean.
							IsColliderActiveAndEnabled(collider)) { 
							if (m_StayingCollisionWraps.TryGetValue(collider, out var collisionWrap)) {
								RaiseCollisionExit(collisionWrap.Collision);
							} else {
								RaiseTriggerExit(collisionTrigger);
							}
						}
						// Remove the associated collition data, if any
						m_StayingCollisionWraps.Remove(collider);
					}
				}
			}

			m_TempStayingColliders.Clear();

		}

		protected void _OnTriggerStay(T_Collider other) {
			m_TempStayingColliders.Add(other);
		}

		protected void _OnCollisionStay(T_Collision other) {
			var collisionWarp = new CollisionWrap(other);
			if (m_TempStayingColliders.Add(collisionWarp.Collider)) {
				// Store the collision info for later usage
				m_StayingCollisionWraps[collisionWarp.Collider] = collisionWarp;
			}
		}

		private void OnDestroy() {
			OnTriggerEnterEv = null;
			OnTriggerExitEv = null;
			OnCollisionEnterEv = null;
			OnCollisionExitEv = null;
		}

		#endregion


		#region Private Fields - Serialized

		[Tooltip("The tag options that will be used in Self Tag and Other Tags.")]
		[SerializeField]
		private StringOptions m_TagOptions;

		[Tooltip("A set of tags for this trigger to be identified by other triggers.")]
		[StringOptions("m_TagOptions")]
		[SerializeField]
		private List<string> m_ThisTags;

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
		protected UnityEvent<T_CollisionTrigger> m_OnTriggerEnter;

		[Tooltip("Raised when another collision trigger with SelfTags that match the OtherTags of this one exits this one.")]
		[UnityEventGroup("TriggerEvents")]
		[SerializeField]
		protected UnityEvent<T_CollisionTrigger> m_OnTriggerExit;

		[Tooltip("Raised when another collision trigger with SelfTags that match the OtherTags of this one enters this one.")]
		[UnityEventGroup("CollisionEvents")]
		[SerializeField]
		protected UnityEvent<T_Collision> m_OnCollisionEnter;

		[Tooltip("Raised when another collision trigger with SelfTags that match the OtherTags of this one exits this one.")]
		[UnityEventGroup("CollisionEvents")]
		[SerializeField]
		protected UnityEvent<T_Collision> m_OnCollisionExit;

		#endregion


		#region Private Fields - Non Serialized

		[NonSerialized]
		private HashSet<T_Collider> m_TempStayingColliders = new HashSet<T_Collider>();

		[NonSerialized]
		private Dictionary<T_Collider, CollisionWrap> m_StayingCollisionWraps = new Dictionary<T_Collider, CollisionWrap>();

		[NonSerialized]
		private HashSet<T_Collider> m_StayingColliders = new HashSet<T_Collider>();

		[NonSerialized]
		Dictionary<T_CollisionTrigger, HashSet<T_Collider>> m_StayingCollisionTriggers =
			new Dictionary<T_CollisionTrigger, HashSet<T_Collider>>();

		#endregion


		#region Private Methods

		private bool EnterCollisionTrigger(T_CollisionTrigger collisionTrigger, T_Collider collider) {
			if (DoOtherTagsMatch(collisionTrigger)) {
				if (!m_StayingCollisionTriggers.ContainsKey(collisionTrigger)) {
					m_StayingCollisionTriggers[collisionTrigger] = new HashSet<T_Collider> { collider };
					return true;
				}
				m_StayingCollisionTriggers[collisionTrigger].Add(collider);
			}
			return false;
		}

		private bool ExitCollisionTrigger(T_CollisionTrigger collisionTrigger, T_Collider collider) {
			if (DoOtherTagsMatch(collisionTrigger)) {
				if (m_StayingCollisionTriggers.ContainsKey(collisionTrigger)) {
					m_StayingCollisionTriggers[collisionTrigger].Remove(collider);
					if (m_StayingCollisionTriggers[collisionTrigger].Count == 0) {
						m_StayingCollisionTriggers.Remove(collisionTrigger);
						return true;
					}
				}
			}
			return false;
		}

		private bool DoOtherTagsMatch(T_CollisionTrigger otherTrigger) {
			foreach (var otherTag in otherTrigger.m_ThisTags) {
				if (m_OtherTags.Contains(otherTag)) {
					return true;
				}
			}
			return false;
		}

		private bool IsColliderActiveAndEnabled(T_Collider collider) {
			if(collider is Collider) {
				var col = collider as Collider;
				return col.enabled && col.gameObject.activeInHierarchy;
			} else if (collider is Collider2D) {
				var col = collider as Collider2D;
				return col.enabled && col.gameObject.activeInHierarchy;
			}
			return false;
		}

		private void RaiseTriggerEnter(T_CollisionTrigger otherTrigger) {
			OnTriggerEnterEv?.Invoke(otherTrigger);
			m_OnTriggerEnter.Invoke(otherTrigger);
		}

		private void RaiseTriggerExit(T_CollisionTrigger otherTrigger) {
			m_OnTriggerExit?.Invoke(otherTrigger);
			OnTriggerExitEv?.Invoke(otherTrigger);
		}

		private void RaiseCollisionEnter(T_Collision collision) {
			OnCollisionEnterEv?.Invoke(collision);
			m_OnCollisionEnter.Invoke(collision);
		}

		private void RaiseCollisionExit(T_Collision collision) {
			OnCollisionExitEv?.Invoke(collision);
			m_OnCollisionExit.Invoke(collision);
		}

		#endregion


		#region CollisionWrap

		public class CollisionWrap {

			public CollisionWrap(T_Collision collision) => m_Collision = collision;

			private T_Collision m_Collision;

			public T_Collider Collider {
				get {
					if (m_Collision is Collision) {
						return (m_Collision as Collision).collider as T_Collider;
					} else if (m_Collision is Collision2D) {
						return (m_Collision as Collision2D).collider as T_Collider;
					}
					return null;
				}
			}

			public T_Collision Collision => m_Collision;

		}

		#endregion


	}

}
