namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using UnityEngine;

	/// <summary>
	/// Triggers collision events when other <see cref="CollisionTrigger2D"/>s enter and exit this one and have
	/// <see cref="ThisTags"/> that match the <see cref="OtherTags"/>.
	/// </summary>
	public class CollisionTrigger2D : CollisionTriggerBase<CollisionTrigger2D, Collider2D, Collision2D, CollisionReaction2D> {


		#region Public Methods

		public override int ReactionsCount => m_Reactions.Count;

		#endregion


		#region Public Methods

		public override void AddReaction(string otherTag) {
			var collisionReaction = new CollisionReaction2D();
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

		public override CollisionReaction2D GetReaction(string otherTag) => m_Reactions.FirstOrDefault(r => r.OtherTag == otherTag);

		public override CollisionReaction2D GetReaction(int index) => m_Reactions[index];

		#endregion


		#region Unity Methods

		private void OnTriggerStay2D(Collider2D other) {
			_OnTriggerStay(other);
		}

		private void OnCollisionStay2D(Collision2D collision) {
			_OnCollisionStay(collision);
		}

		private void OnValidate() {
			for (int i = 0; i < m_Reactions.Count; i++) {
				if (m_Reactions[i] == null) {
					m_Reactions[i] = new CollisionReaction2D();
				}
				m_Reactions[i].CanEditName = false;
				m_Reactions[i].CanDeleteInstance = false;
			}
		}

		#endregion


		#region Private Fields - Serialized

		[Tooltip("The reactions that this collision trigger will have with other collision triggers.")]
		[SerializeField]
		private CompositeList<CollisionReaction2D> m_Reactions = new CompositeList<CollisionReaction2D>();

		#endregion


		#region Private Properties

		protected override CompositeList<CollisionReaction2D> Reactions => m_Reactions;

		#endregion


	}


	#region Small Types

	[Serializable]
	public class CollisionReaction2D : CollisionReactionBase<CollisionTrigger2D, Collision2D> { }

	#endregion


}
