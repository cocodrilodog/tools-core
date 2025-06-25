namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using UnityEngine;

	/// <summary>
	/// Triggers collision events when other <see cref="CollisionTrigger"/>s enter and exit this one and have
	/// <see cref="ThisTags"/> that match the <see cref="OtherTag"/> of the reactions.
	/// </summary>
	public class CollisionTrigger : CollisionTriggerBase<CollisionTrigger, Collider, Collision, CollisionReaction, Vector3> {


		#region Public Methods

		public override int ReactionsCount => m_Reactions.Count;

		#endregion


		#region Public Methods

		public override void AddReaction(string otherTag) {
			var collisionReaction = new CollisionReaction();
			collisionReaction.Name = otherTag;
			collisionReaction.OtherTag = otherTag;
			m_Reactions.Add(collisionReaction);
		}

		public override bool RemoveReaction(string otherTag) {
			var reaction = GetReaction(otherTag);
			if (reaction != null) {
				m_Reactions.Remove(reaction);
				return true;
			}
			return false;
		}

		public override CollisionReaction GetReaction(string otherTag) => m_Reactions.FirstOrDefault(r => r.OtherTag == otherTag);

		public override CollisionReaction GetReaction(int index) => m_Reactions[index];

		// TODO: Test this
		public override bool Raycast(Vector3 origin, Vector3 direction, float maxDistance, params string[] otherTags) {

			var hitsInfo = Physics.RaycastAll(origin, direction, maxDistance);

			for (int i = 0; i < hitsInfo.Length; i++) {
				var otherTrigger = hitsInfo[i].collider.GetComponentInParent<CollisionTrigger>();
				if (otherTrigger != null) {
					foreach (var otherTag in otherTags) {
						if (otherTrigger.ThisTags.Contains(otherTag)) {
							return true;
						}
					}
				}
			}

			return false;

		}

		#endregion


		#region Unity Methods

		private void Awake() => m_Reactions.ForEach(r => r?.RegisterAsReferenceable(this));

		private void OnTriggerEnter(Collider other) => _OnTriggerEnter(other);

		private void OnTriggerExit(Collider other) => _OnTriggerExit(other);

		private void OnCollisionEnter(Collision other) => _OnCollisionEnter(other);

		private void OnCollisionExit(Collision other) => _OnCollisionExit(other);

		private void OnValidate() {
			for (int i = 0; i < m_Reactions.Count; i++) {
				if (m_Reactions[i] == null) {
					m_Reactions[i] = new CollisionReaction();
				}
				m_Reactions[i].CanEditName = false;
				m_Reactions[i].CanDeleteInstance = false;
			}
		}

		private void OnDestroy() => m_Reactions.ForEach(r => r?.UnregisterReferenceable(this));

		#endregion


		#region Private Fields - Serialized

		[Tooltip("The reactions that this collision trigger will have with other collision triggers.")]
		[SerializeField]
		private CompositeList<CollisionReaction> m_Reactions = new CompositeList<CollisionReaction>();

		#endregion


		#region Private Properties

		protected override CompositeList<CollisionReaction> Reactions => m_Reactions;

		#endregion


	}


	#region Small Types

	[Serializable]
	public class CollisionReaction : CollisionReactionBase<CollisionTrigger, Collision> { }

	#endregion


}
