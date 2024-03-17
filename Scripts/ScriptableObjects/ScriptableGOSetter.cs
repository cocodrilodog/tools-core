namespace CocodriloDog.Core {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ScriptableGOSetter : MonoBehaviour {


		#region Unity Methods

		private void Awake() => m_ScriptableGO.Value = gameObject;

		#endregion


		#region Private Fields

		[SerializeField]
        private ScriptableGO m_ScriptableGO;

		#endregion


	}

}