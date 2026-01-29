namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.Serialization;

	/// <summary>
	/// With this component, coroutines can be created in the inspector.
	/// </summary>
	public class CoroutineTrigger : MonoCompositeRoot {


		#region Public Methods

		/// <summary>
		/// Starts the first coroutine in the list.
		/// </summary>
		public void StartCoroutineUnit() {
			m_CoroutineUnits[0]?.StartCoroutine(this);
		}

		/// <summary>
		/// Starts the coroutine with name <paramref name="unitName"/>.
		/// </summary>
		/// <param name="unitName">The name of the <see cref="CoroutineUnit"/>.</param>
		public void StartCoroutineUnit(string unitName) {
			m_CoroutineUnits.FirstOrDefault(b => b.Name == unitName)?.StartCoroutine(this);
		}

		#endregion


		#region Unity Methods

		private void OnEnable() => m_CoroutineUnits.ForEach(cb => cb.RegisterAsReferenceable(this));
		
		private void OnDisable() => m_CoroutineUnits.ForEach(cb => cb.UnregisterReferenceable(this));

		#endregion


		#region Private Fields

		[Tooltip("Objects that have properties of coroutines and fire events accordingly.")]
		[FormerlySerializedAs("m_CoroutineBlocks")]
		[SerializeField]
		private CompositeList<CoroutineUnit> m_CoroutineUnits;

		#endregion


	}

	/// <summary>
	/// Base class for coroutine units. Concrete classes should have properties and events 
	/// relevant to the coroutine that they will start.
	/// </summary>
	[Serializable]
	public abstract class CoroutineUnit : CompositeObject {


		#region Public Methods

		/// <summary>
		/// Starts the coroutine.
		/// </summary>
		/// <param name="monoBehaviour">The MonoBehaviour that starts the coroutine.</param>
		public virtual void StartCoroutine(MonoBehaviour monoBehaviour) { }

		#endregion


	}

	/// <summary>
	/// <see cref="CoroutineUnit"/> that uses a <see cref="WaitForEndOfFrame"/> coroutine.
	/// </summary>
	[Serializable]
	public class WaitForEndOfFrameUnit : CoroutineUnit {


		#region Public Methods

		public override void StartCoroutine(MonoBehaviour monoBehaviour) {

			base.StartCoroutine(monoBehaviour);
			monoBehaviour.StartCoroutine(WaitForEndOfFrame());

			IEnumerator WaitForEndOfFrame() {
				yield return new WaitForEndOfFrame();
				m_OnEndOfFrame.Invoke();
			}

		}

		#endregion


		#region private Fields

		[SerializeField]
		private UnityEvent m_OnEndOfFrame;

		#endregion


	}

	/// <summary>
	/// <see cref="CoroutineUnit"/> that uses a <see cref="WaitForSeconds"/> coroutine.
	/// </summary>
	[Serializable]
	public class WaitForSecondsUnit : CoroutineUnit {


		#region Public Properties

		public override string DisplayName => $"{Name} ({m_Seconds})";

		#endregion


		#region Public Methods

		public override void StartCoroutine(MonoBehaviour monoBehaviour) {

			base.StartCoroutine(monoBehaviour);
			monoBehaviour.StartCoroutine(WaitForSeconds());

			IEnumerator WaitForSeconds() {
				yield return new WaitForSeconds(m_Seconds);
				m_AfterSeconds.Invoke();
			}

		}

		#endregion


		#region private Fields

		[SerializeField]
		private float m_Seconds;

		[SerializeField]
		private UnityEvent m_AfterSeconds;

		#endregion


	}

}