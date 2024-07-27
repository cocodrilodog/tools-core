namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;


	#region Small Types

	[Serializable]
	public class CollisionReactionBase : CompositeObject {
		public override string DefaultName => "[Edit to choose a Tag]";
	}

	#endregion


	[Serializable]
	public class CollisionReactionBase<T_CollisionTrigger, T_Collision> : CollisionReactionBase {


		#region Public Properties

		public string OtherTag {
			get => m_OtherTag;
			set => m_OtherTag = value;
		}

		#endregion


		#region Public Methods

		public void RaiseTriggerEnter(T_CollisionTrigger otherTrigger) {
			OnTriggerEnterEv?.Invoke(otherTrigger);
			m_OnTriggerEnter.Invoke(otherTrigger);
		}

		public void RaiseTriggerExit(T_CollisionTrigger otherTrigger) {
			m_OnTriggerExit?.Invoke(otherTrigger);
			OnTriggerExitEv?.Invoke(otherTrigger);
		}

		public void RaiseCollisionEnter(T_Collision collision) {
			OnCollisionEnterEv?.Invoke(collision);
			m_OnCollisionEnter.Invoke(collision);
		}

		public void RaiseCollisionExit(T_Collision collision) {
			OnCollisionExitEv?.Invoke(collision);
			m_OnCollisionExit.Invoke(collision);
		}

		#endregion


		#region Public Events

		public event Action<T_Collision> OnCollisionEnterEv;

		public event Action<T_Collision> OnCollisionExitEv;

		public event Action<T_CollisionTrigger> OnTriggerEnterEv;

		public event Action<T_CollisionTrigger> OnTriggerExitEv;

		#endregion


		#region Unity Events

		public void OnDestroy() {
			OnCollisionEnterEv = null;
			OnCollisionExitEv = null;
			OnTriggerEnterEv = null;
			OnTriggerExitEv = null;
		}

		#endregion


		#region Private Fields

		[Tooltip(
			"Tag of other triggers that when collide with this one, they will make " +
			"this trigger to raise collision events."
		)]
		[StringOptions("m_TagOptions")]
		[SerializeField]
		private string m_OtherTag;

		[Tooltip(
			"Raised when another collision trigger with SelfTags that match the " +
			"OtherTag of this reaction enters this collision trigger."
		)]
		[UnityEventGroup("CollisionEvents")]
		[SerializeField]
		private UnityEvent<T_Collision> m_OnCollisionEnter;

		[Tooltip(
			"Raised when another collision trigger with SelfTags that match the " +
			"OtherTags of this reaction exits this collision trigger."
		)]
		[UnityEventGroup("CollisionEvents")]
		[SerializeField]
		private UnityEvent<T_Collision> m_OnCollisionExit;

		[Tooltip(
			"Raised when another collision trigger with SelfTags that match the " +
			"OtherTag of this reaction, enters this collision trigger."
		)]
		[UnityEventGroup("TriggerEvents")]
		[SerializeField]
		private UnityEvent<T_CollisionTrigger> m_OnTriggerEnter;

		[Tooltip(
			"Raised when another collision trigger with SelfTags that match the " +
			"OtherTag of this reaction exits this collision trigger."
		)]
		[UnityEventGroup("TriggerEvents")]
		[SerializeField]
		private UnityEvent<T_CollisionTrigger> m_OnTriggerExit;

		#endregion


	}

}