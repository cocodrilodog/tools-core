namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Generic template for <see cref="MonoBehaviour"/> singletons
	/// </summary>
	// TODO: Add abstract keyword, but do it in a project that is tested in many cases.
	public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {


		#region Singleton

		private static T s_Instance;

		/// <summary>
		/// Has the singleton an instance either living or already destroyed?
		/// </summary>
		private static bool s_HasInstance; 

		public static T Instance {
			get {

				// Use the _HasInstance flag instead of comparing the instance to null. This addresses
				// an edge case where the singleton is destroyed before other MonoBehaviour classes
				// which try to call the Instance getter on their respectives OnDestroy(). If that 
				// happened, a new instance would be created because the first one would 
				// compare to null at this point, causing an error. You are not supposed to 
				// instantiate game objects OnDestroy().
				//
				// This is due to the way MonoBehavior implements its == operator:
				// https://blogs.unity3d.com/2014/05/16/custom-operator-should-we-keep-it/
				if (!s_HasInstance) {

					// If the object was created in the editor, it will be found here:
					s_Instance = FindFirstObjectByType<T>(); 

					// If it is still null, create a game object
					if (s_Instance == null) { 
						GameObject gameObject = new GameObject(typeof(T).Name);
						s_Instance = gameObject.AddComponent<T>();
					}

					s_HasInstance = true;

				}

				// It will return null only if it has been created but is already destroyed.
				if (s_Instance.m_IsDestroyed) {
					return null;
				} else {
					return s_Instance;
				}

			}
		}

		#endregion


		#region Public Static Methods

		/// <summary>
		/// Destroys the instance and allows further reinstantiation.
		/// </summary>
		/// <remarks>
		/// Use this method if you want to instantiate the singleton more than 
		/// once in the app lifecycle.
		/// </remarks>
		public static void DestroyForReinstantiation() {
			if (Instance != null) {
				Instance.m_AllowReInstantiationAfterDestroy = true;
				Destroy(Instance.gameObject);
			}
		}

		/// <summary>
		/// Allows further reinstantiation after the instance has been destroyed.
		/// </summary>
		/// <remarks>
		/// Call this before the insance is destroyed if you want to instantiate the 
		/// singleton more than once in the app lifecycle.
		/// </remarks>
		public static void AllowReInstantiation() {
			if(Instance != null) {
				Instance.m_AllowReInstantiationAfterDestroy = true;
			}
		}

		#endregion


		#region Unity Methods

		protected virtual void Awake() {
			// Assign the instance on awake in case an instance that was created
			// manually in the scene is disabled. Otherwise, when calling
			// Instance, it won't be found by FindObjectOfType<T>() while disabled
			// and a new one will be created instead.
			if (!s_HasInstance) {
				s_Instance = this as T;
				s_HasInstance = true;
			}
		}

		protected virtual void OnDestroy() {

			m_IsDestroyed = true;

			if(m_AllowReInstantiationAfterDestroy) {
				m_AllowReInstantiationAfterDestroy = false;
				// This will allow to re-instantiate the singleton next time Instance 
				// is called
				s_HasInstance = false;
			}

		}

		#endregion


		#region Private Fields

		[NonSerialized]
		private bool m_IsDestroyed;

		[NonSerialized]
		private bool m_AllowReInstantiationAfterDestroy;

		#endregion


	}

}
