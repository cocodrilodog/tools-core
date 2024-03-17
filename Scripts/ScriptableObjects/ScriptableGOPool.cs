namespace CocodriloDog.Core {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A pool for game objects.
    /// </summary>
	[CreateAssetMenu(menuName = "Cocodrilo Dog/Core/ScriptableGOPool")]
    public class ScriptableGOPool : ScriptableObject {


        #region Public Methods

        /// <summary>
        /// Gets an object from the pool.
        /// </summary>
        /// <returns>The game object</returns>
        public GameObject Get() {
            GameObject go;
            if (m_InactiveGOs.Count > 0) {
                go = m_InactiveGOs[0];
                m_InactiveGOs.RemoveAt(0);
                m_ActiveGOs.Add(go);
            } else {
                go = Instantiate(m_Prefab);
                //Debug.Log($"Instantiated {go}");
                m_ActiveGOs.Add(go);
            }

            go.SetActive(true);
            return go;
        }

        /// <summary>
        /// Returns a object to the pool.
        /// </summary>
        /// <param name="go">The game object to return</param>
        public void Return(GameObject go) {

            if (!m_ActiveGOs.Contains(go)) {
                throw new ArgumentException($"GameObject {go} does not belong to this {GetType().Name}");
            }

            m_ActiveGOs.Remove(go);
            m_InactiveGOs.Add(go);
            go.SetActive(false);

        }

        #endregion


        #region Private Fields

        [Tooltip("The prefab to instantiate from")]
        [SerializeField]
        private GameObject m_Prefab;

        /// <summary>
        /// Stores the objects that are inactive
        /// </summary>
        [NonSerialized]
        private List<GameObject> m_InactiveGOs = new List<GameObject>();

        /// <summary>
        /// keeps a reference of objects that are in use by the game.
        /// </summary>
        [NonSerialized]
        private List<GameObject> m_ActiveGOs = new List<GameObject>();

        #endregion


    }
}