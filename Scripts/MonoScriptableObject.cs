namespace CocodriloDog.Core {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoScriptableObject : ScriptableObject {


		#region Public Methods

		public void SetOwner(Object owner) {
			m_Owner = owner;
		}

		#endregion


		#region Public Properties

		public Object Owner => m_Owner;

		#endregion


		#region Private Fields

		private Object m_Owner;

		#endregion


	}

}