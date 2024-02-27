namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A pool for game objects.
	/// </summary>
	public class MonoGOPool : MonoBehaviour {


		#region Public Fields

		/// <summary>
		/// The prefab to create clones from.
		/// </summary>
		/// <remarks>
		/// This will only have effect if it is set before <see cref="Start"/>.
		/// </remarks>
		[SerializeField]
		public GameObject Prefab;

		/// <summary>
		/// Will the pool create new instances when empty?
		/// </summary>
		[SerializeField]
		public bool InstantiateWhenEmpty = true;

		#endregion


		#region Public Properties

		/// <summary>
		/// The number of initial instances.
		/// </summary>
		/// <remarks>
		/// This will only have effect if it is set before <see cref="Start"/>.
		/// </remarks>
		public int Count {
			get => m_Count;
			set => m_Count = value;
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Obtains a clone of the <see cref="Prefab"/>.
		/// </summary>
		/// <returns></returns>
		public GameObject GetClone() {
			Initialize();
			if (InactiveClones.Count == 0) {
				if (InstantiateWhenEmpty) {
					InstantiateAndAddClone();
					Debug.LogFormat("{0}: Instantiated new clone of {1}", name, Prefab);
				} else {
					return null;
				}
			}
			GameObject clone = InactiveClones[0];
			InactiveClones.RemoveAt(0);
			ActiveClones.Add(clone);
			clone.SetActive(true);
			return clone;
		}

		/// <summary>
		/// Stores back a clone into the pool.
		/// </summary>
		/// <param name="clone"></param>
		public void DumpClone(GameObject clone) {
			if (ActiveClones.Remove(clone)) {
				InactiveClones.Add(clone);
				clone.SetActive(false);
				// Make it a child again, in case it was taken out while active.
				clone.transform.SetParent(transform);
			} else {
				throw new InvalidOperationException(
					string.Format(
						"The provided GameObject {0} isn't part of this {1} or isn't active yet.",
						clone.name, GetType().Name
					)
				);
			}
		}

		#endregion


		#region Unity Methods

		// Changed this to Start() so that when the pool is created via code, 
		// the prefab and count can be set before it initializes.
		private void Start() {
			Initialize();
		}

		#endregion


		#region Private Fields - Serialized

		[SerializeField]
		private int m_Count = 10;

		#endregion


		#region Private Fields - Non Serialized

		[NonSerialized]
		private bool m_IsInitialized;

		[NonSerialized]
		private List<GameObject> m_InactiveClones;

		[NonSerialized]
		private List<GameObject> m_ActiveClones;

		#endregion


		#region Private Properties

		private List<GameObject> InactiveClones {
			get { return m_InactiveClones = m_InactiveClones ?? new List<GameObject>(); }
		}

		private List<GameObject> ActiveClones {
			get { return m_ActiveClones = m_ActiveClones ?? new List<GameObject>(); }
		}

		#endregion


		#region Private Methods

		private void Initialize() {
			if (!m_IsInitialized) {
				m_IsInitialized = true;
				for (int i = 0; i < m_Count; i++) {
					InstantiateAndAddClone();
				}
			}
		}

		private void InstantiateAndAddClone() {
			GameObject clone = Instantiate(Prefab, transform);
			clone.name = string.Format("{0}{1}", Prefab.name, InactiveClones.Count.ToString());
			clone.SetActive(false);
			InactiveClones.Add(clone);
		}

		#endregion


	}
}