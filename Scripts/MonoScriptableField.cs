namespace CocodriloDog.Core {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class MonoScriptableField<T> where T : MonoScriptableObject {


        #region Private Fields

        [SerializeField]
        private T m_MonoScriptableObject;

		#endregion


	}

}