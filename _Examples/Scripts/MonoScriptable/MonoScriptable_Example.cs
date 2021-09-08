namespace CocodriloDog.Core.Examples {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Example for the MonoScriptable systems.
    /// </summary>
    public class MonoScriptable_Example : MonoBehaviour {


        #region Public Fields

        [SerializeField]
        public MonoScriptableField_Example SomeScriptableField;

        [SerializeField]
        public MonoScriptableField_Example[] SomeScriptableFields;

        #endregion


    }

}