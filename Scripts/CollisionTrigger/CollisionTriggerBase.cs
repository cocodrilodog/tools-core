namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.Serialization;

	/// <summary>
	/// Triggers collision events when other <see cref="ITaggedObject"/>s enter and exit this one and have
	/// <see cref="Tags"/> that match the <see cref="OtherTags"/>.
	/// </summary>
	public abstract class CollisionTriggerBase<T_Collider, T_Collision, T_CollisionReaction, T_Vector> : MonoCompositeRoot, ITaggedObject
		where T_Collider : Component
		where T_Collision : class
		where T_CollisionReaction : CollisionReactionBase<T_Collision> {


		#region Public Properties

		/// <summary>
		/// A set of tags for this trigger to be identified by other triggers.
		/// </summary>
		public List<string> Tags {
			get => m_Tags;
			set => m_Tags = value;
		}

		/// <summary>
		/// Gets the collision triggers that are currently collisioning with this one.
		/// </summary>
		/// <returns>The list</returns>
		public List<ITaggedObject> OtherStaying => m_StayingTaggedObjects.Keys.ToList();

		/// <summary>
		/// The number of reactions that this collision trigger has.
		/// </summary>
		public abstract int ReactionsCount { get; }

		#endregion


		#region Public Methods

		/// <summary>
		/// Adds a reaction that will raise enter and exit events when another tagged object with 
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
		/// Returns <c>true</c> if the provided <paramref name="otherTaggedObject"/> is collisioning with this 
		/// game object, otherwise <c>false</c>.
		/// </summary>
		/// <param name="otherTaggedObject">Other tagged object</param>
		/// <returns>
		/// <c>true</c> if there <paramref name="otherTaggedObject"/> is collisioning with this one, <c>false</c> otherwise.
		/// </returns>
		public bool IsOtherStaying(ITaggedObject otherTaggedObject) => m_StayingTaggedObjects.ContainsKey(otherTaggedObject);

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
			var otherCollisioning = m_StayingTaggedObjects.Keys.FirstOrDefault(t => t.Tags.Contains(otherTag));
			return otherCollisioning != null;
		}

		public abstract bool Raycast(T_Vector origin, T_Vector direction, float maxDistance, params string[] otherTags);

		#endregion


		#region Unity Methods

		protected void _OnTriggerEnter(T_Collider other) {
			var otherTaggedObject = other.GetComponentInParent<ITaggedObject>(true);
			if (otherTaggedObject != null) {
				var reaction = GetMatchingReaction(otherTaggedObject);
				if (reaction != null) {
					if (!m_StayingTaggedObjects.ContainsKey(otherTaggedObject)) {
						m_StayingTaggedObjects[otherTaggedObject] = new List<T_Collider>();
						m_StayingTaggedObjects[otherTaggedObject].Add(other);
						reaction.RaiseTriggerEnter(otherTaggedObject);
					} else {
						m_StayingTaggedObjects[otherTaggedObject].Add(other);
					}
				}
			}
		}

		protected void _OnTriggerExit(T_Collider other) {
			var otherTaggedObject = other.GetComponentInParent<ITaggedObject>(true);
			if (otherTaggedObject != null) {
				var reaction = GetMatchingReaction(otherTaggedObject);
				// Checking the key, handles edge cases when the m_StayingTaggedObjects is cleared
				// prior to OnTriggerExit
				if (reaction != null && m_StayingTaggedObjects.ContainsKey(otherTaggedObject)) {
					m_StayingTaggedObjects[otherTaggedObject].Remove(other);
					if (m_StayingTaggedObjects[otherTaggedObject].Count == 0) {
						m_StayingTaggedObjects.Remove(otherTaggedObject);
						reaction.RaiseTriggerExit(otherTaggedObject);
					}
				}
			}
		}

		protected void _OnCollisionEnter(T_Collision collision) {
			var other = GetColliderFromCollision(collision);
			var otherTaggedObject = other.GetComponentInParent<ITaggedObject>(true);
			if (otherTaggedObject != null) {
				var reaction = GetMatchingReaction(otherTaggedObject);
				if (reaction != null) {
					if (!m_StayingTaggedObjects.ContainsKey(otherTaggedObject)) {
						m_StayingTaggedObjects[otherTaggedObject] = new List<T_Collider>();
						m_StayingTaggedObjects[otherTaggedObject].Add(other);
						reaction.RaiseCollisionEnter(collision);
					} else {
						m_StayingTaggedObjects[otherTaggedObject].Add(other);
					}
				}
			}
		}

		protected void _OnCollisionExit(T_Collision collision) {
			var other = GetColliderFromCollision(collision);
			var otherTaggedObject = other.GetComponentInParent<ITaggedObject>(true);
			if (otherTaggedObject != null) {
				var reaction = GetMatchingReaction(otherTaggedObject);
				// Checking the key, handles edge cases when the m_StayingTaggedObjects is cleared
				// prior to OnCollisionExit
				if (reaction != null && m_StayingTaggedObjects.ContainsKey(otherTaggedObject)) {
					m_StayingTaggedObjects[otherTaggedObject].Remove(other);
					if (m_StayingTaggedObjects[otherTaggedObject].Count == 0) {
						m_StayingTaggedObjects.Remove(otherTaggedObject);
						reaction.RaiseCollisionExit(collision);
					}
				}
			}
		}

		private void Update() {

			// Remove the disabled colliders from the list / dictionary
			List<KeyValuePair<ITaggedObject, List<T_Collider>>> stayingTaggedObjectCollidersToRemove = null;

			// Each list set
			foreach (var stayingTaggedObjectColliders in m_StayingTaggedObjects) {

				List<T_Collider> collidersToRemove = null;

				// Each collider. If the collider is disabled, mark it for removal
				foreach (var collider in stayingTaggedObjectColliders.Value) {
					if (!IsColliderActiveAndEnabled(collider)) {
						collidersToRemove = collidersToRemove ?? new List<T_Collider>();
						collidersToRemove.Add(collider);
					}
				}

				// Remove the collider from the list
				if (collidersToRemove != null) {
					foreach (var collider in collidersToRemove) {
						stayingTaggedObjectColliders.Value.Remove(collider);
					}
				}

				// If the colliders list is empty, mark the collision trigger for removal
				if (stayingTaggedObjectColliders.Value.Count == 0) {
					stayingTaggedObjectCollidersToRemove =
						stayingTaggedObjectCollidersToRemove ?? new List<KeyValuePair<ITaggedObject, List<T_Collider>>>();
					stayingTaggedObjectCollidersToRemove.Add(stayingTaggedObjectColliders);
				}

			}

			// Remove the collision triggers with their empty lists
			if (stayingTaggedObjectCollidersToRemove != null) {
				foreach (var stayingTaggedObjectColliders in stayingTaggedObjectCollidersToRemove) {
					m_StayingTaggedObjects.Remove(stayingTaggedObjectColliders.Key);
				}
			}

		}

		private void OnDisable() {
			// Clear the data only when the game object is disabled, which will turn off the children
			// colliders. If the component is disabled, the only thing that will happen is that the 
			// Update won't be called for a while.
			if (!gameObject.activeInHierarchy) {
				m_StayingTaggedObjects.Clear();
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
		[FormerlySerializedAs("m_ThisTags")]
		[SerializeField]
		private List<string> m_Tags;

		#endregion


		#region Private Fields - Non Serialized

		[NonSerialized]
		Dictionary<ITaggedObject, List<T_Collider>> m_StayingTaggedObjects =
			new Dictionary<ITaggedObject, List<T_Collider>>();

		#endregion


		#region Private Methods

		private T_CollisionReaction GetMatchingReaction(ITaggedObject otherTaggedObject) {
			foreach (var otherTag in otherTaggedObject.Tags) {
				foreach (var reaction in Reactions) {
					if (reaction.OtherTag == otherTag) {
						return reaction;
					}
				}
			}
			return null;
		}

		private bool IsColliderActiveAndEnabled(T_Collider collider) {
			if (collider == null) {
				return false;
			}
			if (collider is Collider) {
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
