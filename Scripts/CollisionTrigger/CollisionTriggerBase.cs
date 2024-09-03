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
	public abstract class CollisionTriggerBase<T_CollisionTrigger, T_Collider, T_Collision, T_CollisionReaction, T_Vector> : CompositeRoot
		where T_CollisionTrigger : CollisionTriggerBase<T_CollisionTrigger, T_Collider, T_Collision, T_CollisionReaction, T_Vector>
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

		/// <summary>
		/// The number of reactions that this collision trigger has.
		/// </summary>
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
		/// <returns>
		/// <c>true</c> if there <paramref name="otherTrigger"/> is collisioning with this one, <c>false</c> otherwise.
		/// </returns>
		public bool IsOtherStaying(T_CollisionTrigger otherTrigger) => m_StayingCollisionTriggers.ContainsKey(otherTrigger);

		/// <summary>
		/// Returns <c>true</c> if there is any collision trigger with self tag <paramref name="otherTag"/>
		/// collisioning with this trigger.
		/// </summary>
		/// <param name="otherTag">Other collision tag.</param>
		/// <returns>
		/// <c>true</c> if there <paramref name="otherTrigger"/> with a ThisTag like <paramref name="otherTag"/> is 
		/// collisioning with this one, <c>false</c> otherwise.
		/// </returns>
		public bool IsOtherStaying(string otherTag) {
			var otherCollisioning = m_StayingCollisionTriggers.Keys.FirstOrDefault(t => t.ThisTags.Contains(otherTag));
			return otherCollisioning != null;
		}

		public abstract bool Raycast(T_Vector origin, T_Vector direction, float maxDistance, params string[] otherTags);

		#endregion


		#region Unity Methods

		protected void _OnTriggerEnter(T_Collider other) {
			var otherCollisionTrigger = other.GetComponentInParent<T_CollisionTrigger>(true);
			if (otherCollisionTrigger != null) {
				var reaction = GetMatchingReaction(otherCollisionTrigger);
				if (reaction != null) {
					if (!m_StayingCollisionTriggers.ContainsKey(otherCollisionTrigger)) {
						m_StayingCollisionTriggers[otherCollisionTrigger] = new List<T_Collider>();
						m_StayingCollisionTriggers[otherCollisionTrigger].Add(other);
						reaction.RaiseTriggerEnter(otherCollisionTrigger);
					} else {
						m_StayingCollisionTriggers[otherCollisionTrigger].Add(other);
					}
				}
			}
		}

		protected void _OnTriggerExit(T_Collider other) {
			var otherCollisionTrigger = other.GetComponentInParent<T_CollisionTrigger>(true);
			if (otherCollisionTrigger != null) {
				var reaction = GetMatchingReaction(otherCollisionTrigger);
				// Checking the key handles edge cases when the m_StayingCollisionTriggers is cleared
				// prior to OnTriggerExit
				if (reaction != null && m_StayingCollisionTriggers.ContainsKey(otherCollisionTrigger)) {
					m_StayingCollisionTriggers[otherCollisionTrigger].Remove(other);
					if (m_StayingCollisionTriggers[otherCollisionTrigger].Count == 0) {
						m_StayingCollisionTriggers.Remove(otherCollisionTrigger);
						reaction.RaiseTriggerExit(otherCollisionTrigger);
					}
				}
			}
		}

		protected void _OnCollisionEnter(T_Collision collision) {
			var other = GetColliderFromCollision(collision);
			var otherCollisionTrigger = other.GetComponentInParent<T_CollisionTrigger>(true);
			if (otherCollisionTrigger != null) {
				var reaction = GetMatchingReaction(otherCollisionTrigger);
				if (reaction != null) {
					if (!m_StayingCollisionTriggers.ContainsKey(otherCollisionTrigger)) {
						m_StayingCollisionTriggers[otherCollisionTrigger] = new List<T_Collider>();
						m_StayingCollisionTriggers[otherCollisionTrigger].Add(other);
						reaction.RaiseCollisionEnter(collision);
					} else {
						m_StayingCollisionTriggers[otherCollisionTrigger].Add(other);
					}
				}
			}
		}

		protected void _OnCollisionExit(T_Collision collision) {
			var other = GetColliderFromCollision(collision);
			var otherCollisionTrigger = other.GetComponentInParent<T_CollisionTrigger>(true);
			if (otherCollisionTrigger != null) {
				var reaction = GetMatchingReaction(otherCollisionTrigger);
				// Checking the key handles edge cases when the m_StayingCollisionTriggers is cleared
				// prior to OnCollisionExit
				if (reaction != null && m_StayingCollisionTriggers.ContainsKey(otherCollisionTrigger)) {
					m_StayingCollisionTriggers[otherCollisionTrigger].Remove(other);
					if (m_StayingCollisionTriggers[otherCollisionTrigger].Count == 0) {
						m_StayingCollisionTriggers.Remove(otherCollisionTrigger);
						reaction.RaiseCollisionExit(collision);
					}
				}
			}
		}

		private void Update() {

			// Remove the disabled colliders from the list / dictionary
			List<KeyValuePair<T_CollisionTrigger, List<T_Collider>>> stayingCollisionTriggerCollidersToRemove = null;

			// Each list set
			foreach (var stayingCollisionTriggerColliders in m_StayingCollisionTriggers) {

				List<T_Collider> collidersToRemove = null;

				// Each collider. If the collider is disabled, mark it for removal
				foreach (var collider in stayingCollisionTriggerColliders.Value) {
					if (!IsColliderActiveAndEnabled(collider)) {
						collidersToRemove = collidersToRemove ?? new List<T_Collider>();
						collidersToRemove.Add(collider);
					}
				}

				// Remove the collider from the list
				if (collidersToRemove != null) {
					foreach (var collider in collidersToRemove) {
						stayingCollisionTriggerColliders.Value.Remove(collider);
					}
				}

				// If the list set is empty, mark it for removal
				if (stayingCollisionTriggerColliders.Value.Count == 0) {
					stayingCollisionTriggerCollidersToRemove =
						stayingCollisionTriggerCollidersToRemove ?? new List<KeyValuePair<T_CollisionTrigger, List<T_Collider>>>();
					stayingCollisionTriggerCollidersToRemove.Add(stayingCollisionTriggerColliders);
				}

			}

			// Remove the list set
			if (stayingCollisionTriggerCollidersToRemove != null) {
				foreach (var stayingCollisionTriggerColliders in stayingCollisionTriggerCollidersToRemove) {
					m_StayingCollisionTriggers.Remove(stayingCollisionTriggerColliders.Key);
				}
			}

		}

		private void OnDisable() {
			// Clear the data only when the game object is disabled, which will turn off the children
			// colliders. If the component is disabled, the only thing that will happen is that the 
			// Update won't be called for a while.
			if (!gameObject.activeInHierarchy) {
				m_StayingCollisionTriggers.Clear();
			}
		}

		private void OnDestroy() {
			foreach (var reaction in Reactions) {
				if (reaction != null) {
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

		[Tooltip("The tag options that will be used in Self Tag and Reactions.")]
		[CreateAsset]
		[SerializeField]
		private StringOptions m_TagOptions;

		[Tooltip("A set of tags for this trigger to be identified by other triggers.")]
		[StringOptions("m_TagOptions")]
		[SerializeField]
		private List<string> m_ThisTags;

		#endregion


		#region Private Fields - Non Serialized

		[NonSerialized]
		Dictionary<T_CollisionTrigger, List<T_Collider>> m_StayingCollisionTriggers =
			new Dictionary<T_CollisionTrigger, List<T_Collider>>();

		#endregion


		#region Private Methods

		private T_CollisionReaction GetMatchingReaction(T_CollisionTrigger otherCollisionTrigger) {
			foreach (var otherTag in otherCollisionTrigger.m_ThisTags) {
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
				var _collider = collider as Collider;
				return _collider.enabled && _collider.gameObject.activeInHierarchy;
			} else if (collider is Collider2D) {
				var _collider = collider as Collider2D;
				return _collider.enabled && _collider.gameObject.activeInHierarchy;
			}
			return false;
		}

		private T_Collider GetColliderFromCollision(T_Collision collision) {
			if (collision is Collision) {
				return (collision as Collision).collider as T_Collider;
			} else if (collision is Collision2D) {
				return (collision as Collision2D).collider as T_Collider;
			}
			return null;
		}

		#endregion


	}

}
