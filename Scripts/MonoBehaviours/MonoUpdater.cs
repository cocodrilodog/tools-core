namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;


	#region Small Types

	public interface IUpdatable {
		void Update();
	}

	public interface IFixedUpdatable {
		void FixedUpdate();
	}

	#endregion


	/// <summary>
	/// Calls Update and FixedUpdate in the added objects, and starts/stops coroutines.
	/// </summary>
	/// 
	/// <remarks>
	/// Use this class on non-MonoBehaviour classes where you want to receive Update and FixedUpdate
	/// calls and create coroutines.
	/// </remarks>
	public class MonoUpdater : MonoSingleton<MonoUpdater> {


		#region Public Static Methods

		/// <summary>
		/// Adds an object that will receive calls to its Update implementation.
		/// </summary>
		/// <param name="updatable">The object.</param>
		public static void AddUpdatable(IUpdatable updatable) {
			if (Instance != null) {
				Instance._AddUpdatable(updatable);
			}
		}

		/// <summary>
		/// Removes an object from receiving calls to its Update implementation.
		/// </summary>
		/// <param name="updatable">The object</param>
		public static void RemoveUpdatable(IUpdatable updatable) {
			if (Instance != null) {
				Instance._RemoveUpdatable(updatable);
			}
		}

		/// <summary>
		/// Adds an object that will receive calls to its FixedUpdate implementation.
		/// </summary>
		/// <param name="fixedUpdatable">The object.</param>
		public static void AddFixedUpdatable(IFixedUpdatable fixedUpdatable) {
			if (Instance != null) {
				Instance._AddFixedUpdatable(fixedUpdatable);
			}
		}

		/// <summary>
		/// Removes an object from receiving calls to its FixedUpdate implementation.
		/// </summary>
		/// <param name="fixedUpdatable">The object.</param>
		public static void RemoveFixedUpdatable(IFixedUpdatable fixedUpdatable) {
			if (Instance != null) {
				Instance._RemoveFixedUpdatable(fixedUpdatable);
			}
		}

		/// <summary>
		/// Starts a coroutine.
		/// </summary>
		/// <param name="routine">The IEnumerator routine.</param>
		/// <returns>The coroutine</returns>
		public static Coroutine StartCoroutine_(IEnumerator routine) {
			if (Instance != null) {
				return Instance.StartCoroutine(routine);
			}
			return null;
		}

		/// <summary>
		/// Stops a coroutine.
		/// </summary>
		/// <param name="routine">The coroutine.</param>
		public static void StopCoroutine_(Coroutine routine) {
			if (Instance != null) {
				Instance.StopCoroutine(routine);
			}
		}

		#endregion


		#region Unity Methods

		protected override void Awake() {
			base.Awake();
			DontDestroyOnLoad(gameObject);
		}

		private void FixedUpdate() {
			foreach (var fixedUpdatable in m_FixedUpdatables) {
				fixedUpdatable.FixedUpdate();
			}
		}

		private void Update() {
			foreach(var updatable in m_Updatables) {
				updatable.Update();
			}
		}

		#endregion


		#region Private Fields

		private List<IUpdatable> m_Updatables = new List<IUpdatable>();

		private List<IFixedUpdatable> m_FixedUpdatables = new List<IFixedUpdatable>();

		#endregion


		#region Private Methods

		private void _AddUpdatable(IUpdatable updatable) => m_Updatables.Add(updatable);

		private void _RemoveUpdatable(IUpdatable updatable) => m_Updatables.Remove(updatable);

		private void _AddFixedUpdatable(IFixedUpdatable fixedUpdatable) => m_FixedUpdatables.Add(fixedUpdatable);

		private void _RemoveFixedUpdatable(IFixedUpdatable fixedUpdatable) => m_FixedUpdatables.Remove(fixedUpdatable);

		#endregion


	}

}