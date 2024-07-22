namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class CollisionTrigger_Example : MonoBehaviour {
		
		public void SetPositionX (float value) {
			var pos = m_RigidBody.position;
			pos.x = value * m_XMultiplier + m_XOffset;
			m_RigidBody.position = pos;
		}

		[SerializeField]
		private Rigidbody m_RigidBody;

		[SerializeField]
		private float m_XMultiplier = 1;

		[SerializeField]
		private float m_XOffset = 0;

	}

}