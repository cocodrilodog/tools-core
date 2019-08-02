namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Generic template for <see cref="MonoBehaviour"/> singletons
	/// </summary>
	public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {


		#region Singleton

		private static T _Instance;

		/// <summary>
		/// Has the singleton an instance either living or already destroyed?
		/// </summary>
		private static bool _HasInstance; 

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
				if (!_HasInstance) {

					// If the object was created in the editor, it will be found here:
					_Instance = FindObjectOfType<T>();

					// If it is still null, create a game object
					if (_Instance == null) { 
						GameObject gameObject = new GameObject(typeof(T).Name);
						_Instance = gameObject.AddComponent<T>();
					}

					_HasInstance = true;

				}

				// It will return null only if it has been created but is already destroyed.
				if (_Instance.m_IsDestroyed) {
					return null;
				} else {
					return _Instance;
				}

			}
		}

		#endregion


		#region Public Static Methods

		/// <summary>
		/// Destroys the instance.
		/// </summary>
		/// <remarks>
		/// Use this method if you want to instantiate the singleton more than 
		/// once in the app lifecycle.
		/// </remarks>
		public static void DestroyInstance() {
			if(Instance != null) {
				Instance.m_AllowReInstantiationAfterDestroy = true;
				Destroy(Instance.gameObject);
			}
		}

		#endregion


		#region MonoBehaviour Methods

		protected virtual void OnDestroy() {

			m_IsDestroyed = true;

			if(m_AllowReInstantiationAfterDestroy) {
				m_AllowReInstantiationAfterDestroy = false;
				// This will allow to re-instantiate the singleton next time Instance 
				// is called
				_HasInstance = false;
			}

		}

		#endregion


		#region Internal Fields

		[NonSerialized]
		private bool m_IsDestroyed;

		[NonSerialized]
		private bool m_AllowReInstantiationAfterDestroy;

		#endregion


	}

}
