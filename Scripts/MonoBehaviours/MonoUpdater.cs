namespace CocodriloDog.Core {

	using System;
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
	/// Calls Update and FixedUpdate in the added objects, and starts/stops coroutines and raises static 
	/// events: <see cref="OnAwakeEv"/>, <see cref="OnEnableEv"/>, <see cref="OnStartEv"/>, 
	/// <see cref="OnDisableEv"/>, and <see cref="OnDestroyEv"/>
	/// </summary>
	/// 
	/// <remarks>
	/// Use this class on non-MonoBehaviour classes where you want to receive Update and FixedUpdate
	/// calls and create coroutines.
	/// </remarks>
	public class MonoUpdater : MonoSingleton<MonoUpdater> {


		#region Public Static Properties

		/// <summary>
		/// Has the singleton awaken?
		/// </summary>
		public static bool IsAwake { get; private set; }

		#endregion


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


		#region Public Static Events

		/// <summary>
		/// Invoked on <see cref="Awake"/> of this <see cref="MonoUpdater"/>.
		/// </summary>
		/// <remarks>
		/// This can be used by processes that happen before awake and need to wait for awakening, such 
		/// as constructors or OnAfterDeserialize.
		/// </remarks>
		public static event Action OnAwakeEv;      // added Ev for consistency

		/// <summary>
		/// Invoked <see cref="OnDestroy"/> of this <see cref="MonoUpdater"/>.
		/// </summary>
		/// <remarks>
		/// This can be used by objects that don't have an OnDestroy when the game finishes, like ScriptableObjects.
		/// </remarks>
		public static event Action OnDestroyEv;    // added Ev to break the conflict with OnDestroy

		#endregion


		#region Unity Methods

		protected override void Awake() {
			base.Awake();
			DontDestroyOnLoad(gameObject);
			IsAwake = true;
			OnAwakeEv?.Invoke();
		}

		private void FixedUpdate() => m_FixedUpdatables.ForEach(fu => fu.FixedUpdate());

		private void Update() => m_Updatables.ForEach(u => u.Update());

		protected override void OnDestroy() {
			base.OnDestroy();
			IsAwake = false;
			OnDestroyEv?.Invoke();
		}

		#endregion


		#region Private Static Methods

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init() {
			_ = Instance; // Force Init as soon as possible
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