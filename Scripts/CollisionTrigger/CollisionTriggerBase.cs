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
	public abstract class CollisionTriggerBase<T_CollisionTrigger, T_Collider, T_Collision, T_CollisionReaction> : CompositeRoot
		where T_CollisionTrigger : CollisionTriggerBase<T_CollisionTrigger, T_Collider, T_Collision, T_CollisionReaction>
		where T_Collider : Component
		where T_Collision : class
		where T_CollisionReaction : CollisionReactionBase<T_CollisionTrigger, T_Collision> {


		#region Public Properties

		/// <summary>
		/// A set of tags for this trigger to be identified by other triggers.
		/// </summary>
		public List<string> ThisTags {
			get => m_ThisTags;
			set => m_ThisTags = value;
		}

		/// <summary>
		/// Gets the collision triggers that are currently collisioning with this one.
		/// </summary>
		/// <returns>The list</returns>
		public List<T_CollisionTrigger> OtherStaying => m_StayingCollisionTriggers.Keys.ToList();

		public abstract int ReactionsCount { get; }

		#endregion


		#region Public Methods

		/// <summary>
		/// Adds a reaction that will raise enter and exit events when another collider with 
		/// tag <paramref name="otherTag"/> enters or exits this <see cref="CollisionTrigger"/>.
		/// </summary>
		/// <param name="otherTag"><c>OtherTag</c> to be assigned to the created reaction.</param>
		public abstract void AddReaction(string otherTag);

		/// <summary>
		/// Removes a reaction with tag <paramref name="otherTag"/>.
		/// </summary>
		/// <param name="otherTag">The <c>OtherTag</c> of the reactoin to remove.</param>
		/// <returns><c>true</c> if the reaction was found and removed, false otherwise.</returns>
		public abstract bool RemoveReaction(string otherTag);

		/// <summary>
		/// Gets a reaction with the specified <paramref name="otherTag"/>.
		/// </summary>
		/// <param name="otherTag">The <c>otherTag</c></param>
		/// <returns>The reaction.</returns>
		public abstract T_CollisionReaction GetReaction(string otherTag);

		/// <summary>
		/// Gets the reaction at the specified <paramref name="index"/>.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The reaction.</returns>
		public abstract T_CollisionReaction GetReaction(int index);

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


		#region Unity Methods

		private void FixedUpdate() {

			// Add the latest staying colliders
			foreach (var collider in m_TempStayingColliders) {
				if (m_StayingColliders.Add(collider)) {
					if (m_DebugColliderCount) {
						Debug.Log($"{name}: + {collider.name}");
					}
					var collisionTrigger = collider.GetComponentInParent<T_CollisionTrigger>(true);
					if (collisionTrigger != null) {
						TryEnterCollisionTrigger(collisionTrigger, collider);
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
						if (m_DebugColliderCount) {
							Debug.Log($"{name}: - {collider.name}");
						}
						// TODO: This would be the place for granular OnExit
						var collisionTrigger = collider.GetComponentInParent<T_CollisionTrigger>(true);
						if (collisionTrigger != null) {
							TryExitCollisionTrigger(collisionTrigger, collider);
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
			m_TempStayingColliders.Add(collisionWarp.Collider);
			// Store the collision info for later usage. Update it always so that it is updated on exit
			m_StayingCollisionWraps[collisionWarp.Collider] = collisionWarp;
		}

		private void OnDisable() {
			m_StayingColliders.Clear();
			m_StayingCollisionWraps.Clear();
			m_StayingCollisionTriggers.Clear();
		}

		private void OnDestroy() {
			foreach(var reaction in Reactions) {
				if(reaction != null) {
					reaction.OnDestroy();
				}
			}
		}

		#endregion


		#region Protected Methods

		/// <summary>
		/// The reactions that this collision trigger will have with other collision triggers.
		/// </summary>
		protected virtual CompositeList<T_CollisionReaction> Reactions => null;

		#endregion


		#region Private Fields - Serialized

		[Tooltip("The tag options that will be used in Self Tag and Other Tags.")]
		[SerializeField]
		private StringOptions m_TagOptions;

		[Tooltip("A set of tags for this trigger to be identified by other triggers.")]
		[StringOptions("m_TagOptions")]
		[SerializeField]
		private List<string> m_ThisTags;

		[HideInInspector]
		[SerializeField]
		private bool m_DebugColliderCount;

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

		/// <summary>
		/// Tries to enter the <paramref name="collisionTrigger"/> given that the collision was triggered
		/// by the <paramref name="collider"/>. 
		/// </summary>
		/// 
		/// <remarks>
		/// If this is the first time that the <paramref name="collisionTrigger"/> is trying to enter, it will enter and it
		/// will start counting the colliders that belong to the <paramref name="collisionTrigger"/>. It will then search 
		/// for a reaction whose tag is equal to any of the <see cref="m_ThisTags"/> of the entering <paramref name="collisionTrigger"/>.
		/// 
		/// If a reaction is found, it will raise an <c>OnCollisionEnter</c> when there is a <see cref="CollisionWrap"/>
		/// for the provided <paramref name="collider"/>. If a reaction was found, but there is no <see cref="CollisionWrap"/>
		/// for the <paramref name="collider"/>, then <c>OnTriggerEnter</c> is raised.
		/// </remarks>
		/// 
		/// <param name="collisionTrigger">The collision trigger.</param>
		/// <param name="collider">The collider that triggered the process.</param>
		/// 
		/// <returns>
		/// <c>true</c> when the <paramref name="collisionTrigger"/> enters for the first time, <c>false</c> otherwise
		/// </returns>
		private bool TryEnterCollisionTrigger(T_CollisionTrigger collisionTrigger, T_Collider collider) {
			var reaction = GetMatchingReaction(collisionTrigger);
			if (reaction != null) {
				if (!m_StayingCollisionTriggers.ContainsKey(collisionTrigger)) {
					m_StayingCollisionTriggers[collisionTrigger] = new HashSet<T_Collider> { collider };
					if (m_StayingCollisionWraps.TryGetValue(collider, out var collisionWrap)) {
						reaction.RaiseCollisionEnter(collisionWrap.Collision);
					} else {
						reaction.RaiseTriggerEnter(collisionTrigger);
					}
					return true;
				}
				m_StayingCollisionTriggers[collisionTrigger].Add(collider);
			}
			return false;
		}

		/// <summary>
		/// Tries to exit the <paramref name="collisionTrigger"/> given that the exit was triggered
		/// by the <paramref name="collider"/>. 
		/// </summary>
		/// 
		/// <remarks>
		/// If this is the last collider that is belongs to the <paramref name="collisionTrigger"/>, this will 
		/// stop counting its colliders, and the <paramref name="collisionTrigger"/> will exit. It will then search for a
		/// reaction whose tag is equal to any of the <see cref="m_ThisTags"/> of the exiting <paramref name="collisionTrigger"/>.
		/// 
		/// If a reaction is found and the involded <paramref name="collider"/> is active and enabled, it will raise an 
		/// <c>OnCollisionExit</c> when there is a <see cref="CollisionWrap"/> for the provided <paramref name="collider"/>.
		/// If a reaction was found and the <paramref name="collider"/> is active and enabled, but there is no 
		/// <see cref="CollisionWrap"/> for the <paramref name="collider"/>, then <c>OnTriggerExit</c> is raised.
		/// </remarks>
		/// 
		/// <param name="collisionTrigger">The collision trigger.</param>
		/// <param name="collider">The collider that belongs to the collision trigger.</param>
		/// 
		/// <returns>
		/// <c>true</c> when the last collider of the <paramref name="collisionTrigger"/> exits, <c>false</c> otherwise.
		/// </returns>
		private bool TryExitCollisionTrigger(T_CollisionTrigger collisionTrigger, T_Collider collider) {
			var reaction = GetMatchingReaction(collisionTrigger);
			if (reaction != null) {
				if (m_StayingCollisionTriggers.ContainsKey(collisionTrigger)) {
					m_StayingCollisionTriggers[collisionTrigger].Remove(collider);
					if (m_StayingCollisionTriggers[collisionTrigger].Count == 0) {
						m_StayingCollisionTriggers.Remove(collisionTrigger);
						// This prevents the on exit to be caused by a collider that was deactivated, 
						// but we are still keeping the colliders count clean by removing it anyway
						if (IsColliderActiveAndEnabled(collider)) {
							if (m_StayingCollisionWraps.TryGetValue(collider, out var collisionWrap)) {
								reaction.RaiseCollisionExit(collisionWrap.Collision);
							} else {
								reaction.RaiseTriggerExit(collisionTrigger);
							}
						}
					}
					return true;
				}
			}
			return false;
		}

		private T_CollisionReaction GetMatchingReaction(T_CollisionTrigger otherTrigger) {
			foreach (var otherTag in otherTrigger.m_ThisTags) {
				foreach(var reaction in Reactions) {
					if(reaction.OtherTag == otherTag) {
						return reaction;
					}
				}
			}
			return null;
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

		#endregion


		#region CollisionWrap

		/// <summary>
		/// Wrapper class for <see cref="T_Collision"/> that helps to get the corresponding <see cref="Collider"/>.
		/// </summary>
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
