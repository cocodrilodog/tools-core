namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.Serialization;

	public class CoroutineTrigger : MonoCompositeRoot {


		#region Public Methods

		public void StartCoroutineUnit() {
			m_CoroutineUnits[0]?.StartCoroutine(this);
		}

		public void StartCoroutineUnit(string unitName) {
			m_CoroutineUnits.FirstOrDefault(b => b.Name == unitName)?.StartCoroutine(this);
		}

		#endregion


		#region Unity Methods

		private void OnEnable() => m_CoroutineUnits.ForEach(cb => cb.RegisterAsReferenceable(this));
		
		private void OnDisable() => m_CoroutineUnits.ForEach(cb => cb.UnregisterReferenceable(this));

		#endregion


		#region Private Fields

		[FormerlySerializedAs("m_CoroutineBlocks")]
		[SerializeField]
		private CompositeList<CoroutineUnit> m_CoroutineUnits;

		#endregion


	}

	[Serializable]
	public abstract class CoroutineUnit : CompositeObject {


		#region Public Methods

		public virtual void StartCoroutine(MonoBehaviour monoBehaviour) { }

		#endregion


	}

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