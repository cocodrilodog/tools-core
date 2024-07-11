namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Triggers collision events when other <see cref="T_Trigger"/>s enter and exit this one and have
	/// <see cref="SelfTags"/> that match the <see cref="OtherTags"/>.
	/// </summary>
	public abstract class CollisionTriggerBase<T_Trigger, T_Collision> : MonoBehaviour 
		where T_Trigger : CollisionTriggerBase<T_Trigger, T_Collision> {


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
		/// Gets the collision triggers that are currently collisioning with this one.
		/// </summary>
		/// <returns>A read-only collection with the triggers.</returns>
		public ReadOnlyCollection<T_Trigger> OtherCollisioning =>
			m_OtherCollisioning_RO = m_OtherCollisioning_RO ?? new ReadOnlyCollection<T_Trigger>(m_OtherCollisioning);

		#endregion


		#region Public Methods

		/// <summary>
		/// Returns <c>true</c> if the provided <paramref name="otherTrigger"/> is collisioning with this 
		/// game object, otherwise <c>false</c>.
		/// </summary>
		/// <param name="otherTrigger">Other collision trigger</param>
		/// <returns></returns>
		public bool IsOtherCollisioning(T_Trigger otherTrigger) => m_OtherCollisioning.Contains(otherTrigger);

		/// <summary>
		/// Returns <c>true</c> if there is any collision trigger with self tag <paramref name="otherTag"/>
		/// collisioning with this trigger.
		/// </summary>
		/// <param name="otherTag"></param>
		/// <returns></returns>
		public bool IsOtherCollisioning(string otherTag) {
			var otherCollisioning = m_OtherCollisioning.FirstOrDefault(t => t.SelfTags.Contains(otherTag));
			return otherCollisioning != null;
		}

		#endregion


		#region Public Events

		public event Action<T_Trigger> OnTriggerEnterEv;

		public event Action<T_Trigger> OnTriggerExitEv;

		public event Action<T_Collision> OnCollisionEnterEv;

		public event Action<T_Collision> OnCollisionExitEv;

		#endregion


		#region Unity Methods

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
		protected UnityEvent<T_Trigger> m_OnTriggerEnter;

		[Tooltip("Raised when another collision trigger with SelfTags that match the OtherTags of this one exits this one.")]
		[UnityEventGroup("TriggerEvents")]
		[SerializeField]
		protected UnityEvent<T_Trigger> m_OnTriggerExit;

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
		private Dictionary<T_Trigger, int> m_OtherCollisioningCount = new Dictionary<T_Trigger, int>();

		[NonSerialized]
		private List<T_Trigger> m_OtherCollisioning = new List<T_Trigger>();

		[NonSerialized]
		private ReadOnlyCollection<T_Trigger> m_OtherCollisioning_RO;

		#endregion


		#region Protected Methods

		/// <summary>
		/// Checks if any of the <see cref="m_OtherTags"/> of this trigger match any of the 
		/// <see cref="m_SelfTags"/> of the other trigger.
		/// </summary>
		/// <param name="otherTrigger">Another collision trigger</param>
		/// <returns><c>true</c> if there is a match, <c>false</c> otherwise.</returns>
		protected bool DoOtherTagsMatch(T_Trigger otherTrigger) {
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
		protected bool EnterTrigger(T_Trigger otherTrigger) {
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
		protected bool ExitTrigger(T_Trigger otherTrigger) {
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

		/// <summary>
		/// Raises the C# OnTriggerEnterEv;
		/// </summary>
		/// <param name="otherTrigger">The collision trigger that enters this one</param>
		protected void RaiseTriggerEnter(T_Trigger otherTrigger) => OnTriggerEnterEv?.Invoke(otherTrigger);

		/// <summary>
		/// Raises the C# OnTriggerExitEv;
		/// </summary>
		/// <param name="otherTrigger">The collision trigger that exits this one</param>
		protected void RaiseTriggerExit(T_Trigger otherTrigger) => OnTriggerExitEv?.Invoke(otherTrigger);

		/// <summary>
		/// Raises the C# OnCollisionEnterEv;
		/// </summary>
		/// <param name="collision">The collision data</param>
		protected void RaiseCollisionEnter(T_Collision collision) => OnCollisionEnterEv?.Invoke(collision);

		/// <summary>
		/// Raises the C# OnCollisionExitEv;
		/// </summary>
		/// <param name="collision">The collision data</param>
		protected void RaiseCollisionExit(T_Collision collision) => OnCollisionExitEv?.Invoke(collision);

		#endregion


	}

}
